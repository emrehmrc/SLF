
##### kullanılan paketler #####
packages <- c("Rcpp", "zip", "readr", "tidyverse", "readxl", "magrittr", "ggplot2", "gridExtra", "stringr", "car", "openxlsx", "jsonlite",
              "scales", "caret", "lattice", "MASS", "dplyr", "forecast", "this.path", "Metrics", "boot", "glmnet", "zoo", "elasticnet")

# paketler yüklü değilse yükle, yüklüyse içeri aktar
for (i in 1:length(packages)) {
  
  if(packages[i] %in% rownames(installed.packages())){
    
    library(packages[i], character.only = TRUE)
    
  } else {
    print(paste0("Paket ismi: ", packages[i], " yukleniyor..."))
    install.packages(packages[i], repos = "http://cran.us.r-project.org", dependencies = TRUE)
    library(packages[i], character.only = TRUE)
    print(paste0("Paket yuklendi!"))
  }
}


# encoding'i Türkçe'ye ayarlar
Sys.setlocale("LC_CTYPE", "tr_TR.UTF-8")

# CLI deki argumentlerin tutulacağı objenin oluşturulması
args <- commandArgs(trailingOnly = TRUE)

# çalışma klasörünü current klasöre ayarla
setwd(this.dir())

# uyarıları süpres et
options(warn = -1)


##### Config dosyasını oku #####
#config <- jsonlite::fromJSON(args[1])

config <- jsonlite::fromJSON("C:/Users/ehan0/MRC/MRC - 1.1.3_T&SI/MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı/il_ilce_kırılımları/Program Dosyaları/config.json")


user_root_path <- Sys.getenv("USERPROFILE")

excel_file_path <- paste0(user_root_path,"/",
                          config$Ana_Klasör_Yolu, "/",
                          config$İl, "/",
                          config$İlçe, "/",
                          config$ELF$INPUT_FILE)


##### Ana girdi dosyasını yükle #####
d <- read_excel(excel_file_path,
                sheet = 1,
                col_names = TRUE,
                col_types = rep("numeric", 43)) %>%
  as.data.frame()


# Horizon years
h <- config$ELF$ufuk_yılı


#   --------------------------------------------  #
# function to extract only the filled cells from the original data
filled_cells <- function(d, t){
  
  d_filled <- d[which(is.na(d[,t]) == FALSE), t]
  return(d_filled)
}

# function to do the necessary data pre-processing
data_preparation <- function(d){
  d <- as.data.frame(d)
  d %>%
    rename("faturalanan" = 1) %>%
    mutate(faturalanan = as.numeric(faturalanan)) -> d
}

# training set length for consumption values
nrow_dataset_tuketim <- data_preparation(filled_cells(d, 7)) %>% nrow()
last_known_year_nufus <- max(d$YIL[!is.na(d$ILCE_NUFUS)]) # Last year with known population data

# the index of the last nufus/faturalanan data
last_known_year_nufus_index <- which(d$YIL == last_known_year_nufus)


# ------------------------- create the datatables that will be used in different scenarios --------------- #

d_minimum <- d
d_dusuk <- d
d_yuksek <- d
d_maximum <- d


# ------------------------- GDP GROWTH projection --------------- #

# Find the last GDP_BUYUME_ORANI value (in 2029 in this case)
nrow_dataset_GDP <- data_preparation(filled_cells(d, 2)) %>% nrow()
last_known_year_gdp_index_growth <- max(which(!is.na(d$GDP_BUYUME_ORANI)))
last_growth_value <- d$GDP_BUYUME_ORANI[last_known_year_gdp_index_growth]

# Determine the last year to be imputed (2023 + h + 1)
end_year <- last_known_year_nufus + h + 1


# Loop through the years from the next available year to the target year and impute the GDP_BUYUME_ORANI value
for (i in (last_known_year_gdp_index_growth + 1):nrow(d)) {
  if (d$YIL[i] <= end_year) {
    d$GDP_BUYUME_ORANI[i] <- last_growth_value
  }
}

d_minimum$GDP_BUYUME_ORANI[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1)] <- d$GDP_BUYUME_ORANI[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1)] - 0.02
d_dusuk$GDP_BUYUME_ORANI[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1)] <- d$GDP_BUYUME_ORANI[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1)] - 0.01
d_yuksek$GDP_BUYUME_ORANI[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1)] <- d$GDP_BUYUME_ORANI[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1)] + 0.01
d_maximum$GDP_BUYUME_ORANI[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1)] <- d$GDP_BUYUME_ORANI[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1)] + 0.02

# ------------------------- KKO-KKM projection --------------- #


for (i in (last_known_year_nufus + 1):(last_known_year_nufus + h + 1)) {
  # Calculate the moving average of percentage changes for the last 7 years (or available years)
  available_years <- (i - 7):(i - 1)
  
  # Calculate moving average for valid years
  kko_values <- na.omit(d$KKO[d$YIL %in% available_years])
  # Check if there are any valid values to calculate the mean
  if (length(kko_values) > 0) {
    moving_avg_change <- mean(kko_values)
  } else {
    # If no valid data, use a default value (e.g., 0) or the last known KKO value
    moving_avg_change <- ifelse(
      any(!is.na(d$KKO[1:(i-1)])),
      tail(na.omit(d$KKO[1:(i-1)]), 1),  # Use the last known non-NA KKO value
      0  # Fallback to 0 if no previous values exist
    )
  }
  
  # Assign the moving average to the KKO column
  d$KKO[d$YIL == i] <- moving_avg_change
  d_minimum$KKO[d$YIL == i] <- moving_avg_change
  d_dusuk$KKO[d$YIL == i] <- moving_avg_change
  d_yuksek$KKO[d$YIL == i] <- moving_avg_change
  d_maximum$KKO[d$YIL == i] <- moving_avg_change
}

# Debug: Print KKO values before creating scenario data frames
print("KKO values in d:")
print(d$KKO)


# -----------------------------GDP and GRP values projection -------------------------------------- #


# fill all the GDP/GRP values until the last possible value used in the train data.
last_known_year_gdp_index <- max(which(!is.na(d$GDP)))

d$GDP[last_known_year_gdp_index + 1] <- d$GDP[last_known_year_gdp_index] * (1 + d$GDP_BUYUME_ORANI[last_known_year_gdp_index+1])
d_minimum$GDP[last_known_year_gdp_index + 1] <- d_minimum$GDP[last_known_year_gdp_index] * (1 + d_minimum$GDP_BUYUME_ORANI[last_known_year_gdp_index+1])
d_dusuk$GDP[last_known_year_gdp_index + 1] <- d_dusuk$GDP[last_known_year_gdp_index] * (1 + d_dusuk$GDP_BUYUME_ORANI[last_known_year_gdp_index+1])
d_yuksek$GDP[last_known_year_gdp_index + 1] <- d_yuksek$GDP[last_known_year_gdp_index] * (1 + d_yuksek$GDP_BUYUME_ORANI[last_known_year_gdp_index+1])
d_maximum$GDP[last_known_year_gdp_index + 1] <- d_maximum$GDP[last_known_year_gdp_index] * (1 + d_maximum$GDP_BUYUME_ORANI[last_known_year_gdp_index+1])

# Loop through the years starting from the year after the last known value
for (i in (last_known_year_gdp_index + 2):(which(d$YIL == end_year))) {
  
  # Impute GRP value using the last known GRP and applying the growth factor
  d$GDP[i] <- d$GDP[i-1] * (1 + d$GDP_BUYUME_ORANI[i])
  d_minimum$GDP[i] <- d_minimum$GDP[i-1] * (1 + d_minimum$GDP_BUYUME_ORANI[i])
  d_dusuk$GDP[i] <- d_dusuk$GDP[i-1] * (1 + d_dusuk$GDP_BUYUME_ORANI[i])
  d_yuksek$GDP[i] <- d_yuksek$GDP[i-1] * (1 + d_yuksek$GDP_BUYUME_ORANI[i])
  d_maximum$GDP[i] <- d_maximum$GDP[i-1] * (1 + d_maximum$GDP_BUYUME_ORANI[i])
  
}


# Columns to impute
columns_to_impute_perc <- c("GDP_TARIMSAL_URETIM_%", "GDP_SANAYI_URETIM_%", "GDP_HIZMET_URETIM_%", "GDP_INSAAT_URETIM_%",
                            "GRP_TARIMSAL_URETIM_%", "GRP_SANAYI_URETIM_%", "GRP_HIZMET_URETIM_%", "GRP_INSAAT_URETIM_%", "ILCE_NUFUS")


for (col in columns_to_impute_perc){
  
  d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),col] <- forecast(ets(ts(d[1:last_known_year_nufus_index,col], 
                                                                                        start = 2013, frequency = 1), damped = TRUE),h+1)$mean
  
  d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),col] <- forecast(ets(ts(d[1:last_known_year_nufus_index,col], 
                                                                                            start = 2013, frequency = 1), damped = TRUE),h+1)$lower[,2]
  
  d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),col] <- forecast(ets(ts(d[1:last_known_year_nufus_index,col], 
                                                                                            start = 2013, frequency = 1), damped = TRUE),h+1)$lower[,1]
  
  d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),col] <- forecast(ets(ts(d[1:last_known_year_nufus_index,col], 
                                                                                            start = 2013, frequency = 1), damped = TRUE),h+1)$upper[,1]
  
  d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),col] <- forecast(ets(ts(d[1:last_known_year_nufus_index,col], 
                                                                                            start = 2013, frequency = 1), damped = TRUE),h+1)$upper[,2]
  
  
}



# normzalize the GDP percantages so it adds up to 1
gdp_percentages <- c("GDP_TARIMSAL_URETIM_%", "GDP_SANAYI_URETIM_%", "GDP_HIZMET_URETIM_%", "GDP_INSAAT_URETIM_%")

# Normalize percentages for each year in the forecasting period
for (i in (last_known_year_nufus_index + 1):(which(d$YIL == end_year))) {
  
  # Normalize GDP percentages
  gdp_total_d <- sum(d[i, gdp_percentages], na.rm = TRUE)
  gdp_total_d_minimum <- sum(d_minimum[i, gdp_percentages], na.rm = TRUE)
  gdp_total_d_dusuk <- sum(d_dusuk[i, gdp_percentages], na.rm = TRUE)
  gdp_total_d_yuksek <- sum(d_yuksek[i, gdp_percentages], na.rm = TRUE)
  gdp_total_d_maximum <- sum(d_maximum[i, gdp_percentages], na.rm = TRUE)
  
  for (col in gdp_percentages) {
    d[i, col] <- d[i, col] / gdp_total_d
    d_minimum[i, col] <- d_minimum[i, col] / gdp_total_d_minimum
    d_dusuk[i, col] <- d_dusuk[i, col] / gdp_total_d_dusuk
    d_yuksek[i, col] <- d_yuksek[i, col] / gdp_total_d_yuksek
    d_maximum[i, col] <- d_maximum[i, col] / gdp_total_d_maximum
  }
  
}



columns_to_impute_monetary <- c("GDP_TARIMSAL_URETIM", "GDP_SANAYI_URETIM", "GDP_HIZMET_URETIM", "GDP_INSAAT_URETIM",
                                "GRP_TARIMSAL_URETIM", "GRP_SANAYI_URETIM", "GRP_HIZMET_URETIM", "GRP_INSAAT_URETIM")

# calculate the sectoral monetary amount projections
for (i in (last_known_year_gdp_index + 1):(which(d$YIL == end_year))) {
  for (cols in columns_to_impute_monetary){
    
    if(substr(cols,1,3) == "GDP"){
      
      d[i, cols] = d[i, "GDP"] * d[i, paste0(cols,"_%")]
      d_minimum[i, cols] = d_minimum[i, "GDP"] * d_minimum[i, paste0(cols,"_%")]
      d_dusuk[i, cols] = d_dusuk[i, "GDP"] * d_dusuk[i, paste0(cols,"_%")]
      d_yuksek[i, cols] = d_yuksek[i, "GDP"] * d_yuksek[i, paste0(cols,"_%")]
      d_maximum[i, cols] = d_maximum[i, "GDP"] * d_maximum[i, paste0(cols,"_%")]
      
    } else {
      
      d[i, cols] = d[i, paste0("GDP_", substr(cols, 5 , 22))] * d[i, paste0("GRP_", substr(cols, 5 , 22) , "_%")]
      d_minimum[i, cols] = d_minimum[i, paste0("GDP_", substr(cols, 5 , 22))] * d_minimum[i, paste0("GRP_", substr(cols, 5 , 22) , "_%")]
      d_dusuk[i, cols] = d_dusuk[i, paste0("GDP_", substr(cols, 5 , 22))] * d_dusuk[i, paste0("GRP_", substr(cols, 5 , 22) , "_%")]
      d_yuksek[i, cols] = d_yuksek[i, paste0("GDP_", substr(cols, 5 , 22))] * d_yuksek[i, paste0("GRP_", substr(cols, 5 , 22) , "_%")]
      d_maximum[i, cols] = d_maximum[i, paste0("GDP_", substr(cols, 5 , 22))] * d_maximum[i, paste0("GRP_", substr(cols, 5 , 22) , "_%")]
      
    }
  }
  
  d[i , "GRP"] = d[i, "GRP_TARIMSAL_URETIM"] + d[i, "GRP_SANAYI_URETIM"] + d[i, "GRP_HIZMET_URETIM"] + d[i, "GRP_INSAAT_URETIM"]
  d_minimum[i , "GRP"] = d_minimum[i, "GRP_TARIMSAL_URETIM"] + d_minimum[i, "GRP_SANAYI_URETIM"] + d_minimum[i, "GRP_HIZMET_URETIM"] + d_minimum[i, "GRP_INSAAT_URETIM"]
  d_dusuk[i , "GRP"] = d_dusuk[i, "GRP_TARIMSAL_URETIM"] + d_dusuk[i, "GRP_SANAYI_URETIM"] + d_dusuk[i, "GRP_HIZMET_URETIM"] + d_dusuk[i, "GRP_INSAAT_URETIM"]
  d_yuksek[i , "GRP"] = d_yuksek[i, "GRP_TARIMSAL_URETIM"] + d_yuksek[i, "GRP_SANAYI_URETIM"] + d_yuksek[i, "GRP_HIZMET_URETIM"] + d_yuksek[i, "GRP_INSAAT_URETIM"]
  d_maximum[i , "GRP"] = d_maximum[i, "GRP_TARIMSAL_URETIM"] + d_maximum[i, "GRP_SANAYI_URETIM"] + d_maximum[i, "GRP_HIZMET_URETIM"] + d_maximum[i, "GRP_INSAAT_URETIM"]
  
}


# ---------------------------------------------CDD_HDD Projections------------------------------------------- #

d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"CDD"] <- forecast(ets(ts(d[1:last_known_year_nufus_index,"CDD"], 
                                                                                              start = 2013, frequency = 1), damped = TRUE), h+1)$mean
d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"CDD"] <- forecast(ets(ts(d_minimum[1:last_known_year_nufus_index,"CDD"], 
                                                                                                      start = 2013, frequency = 1), damped = TRUE), h+1)$upper[,2]
d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"CDD"] <- forecast(ets(ts(d_dusuk[1:last_known_year_nufus_index,"CDD"], 
                                                                                                    start = 2013, frequency = 1), damped = TRUE), h+1)$upper[,1]
d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"CDD"] <- forecast(ets(ts(d_yuksek[1:last_known_year_nufus_index,"CDD"], 
                                                                                                     start = 2013, frequency = 1), damped = TRUE), h+1)$lower[,1]
d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"CDD"] <- forecast(ets(ts(d_maximum[1:last_known_year_nufus_index,"CDD"], 
                                                                                                      start = 2013, frequency = 1), damped = TRUE), h+1)$lower[,2]



d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"HDD"] <- forecast(ets(ts(d[1:last_known_year_nufus_index,"HDD"], 
                                                                                              start = 2013, frequency = 1), damped = TRUE), h+1)$mean
d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"HDD"] <- forecast(ets(ts(d_minimum[1:last_known_year_nufus_index,"HDD"], 
                                                                                                      start = 2013, frequency = 1), damped = TRUE), h+1)$lower[,2]
d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"HDD"] <- forecast(ets(ts(d_dusuk[1:last_known_year_nufus_index,"HDD"], 
                                                                                                    start = 2013, frequency = 1), damped = TRUE), h+1)$lower[,1]
d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"HDD"] <- forecast(ets(ts(d_yuksek[1:last_known_year_nufus_index,"HDD"], 
                                                                                                     start = 2013, frequency = 1), damped = TRUE), h+1)$upper[,1]
d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"HDD"] <- forecast(ets(ts(d_maximum[1:last_known_year_nufus_index,"HDD"], 
                                                                                                      start = 2013, frequency = 1), damped = TRUE), h+1)$upper[,2]


# ---------------------------------------------SENARYOLAR------------------------------------------- #


senaryolar_minimum <- as.data.frame(cbind(YIL = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"YIL"],
                                        GDP_BUYUME_ORANI = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_BUYUME_ORANI"],
                                        ILCE_NUFUS = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"ILCE_NUFUS"],
                                        GDP = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP"],
                                        GDP_TARIMSAL_URETIM = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_TARIMSAL_URETIM"],
                                        GDP_SANAYI_URETIM = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_SANAYI_URETIM"],
                                        GDP_HIZMET_URETIM = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_HIZMET_URETIM"],
                                        GDP_INSAAT_URETIM = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_INSAAT_URETIM"],
                                        GRP = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP"],
                                        GRP_TARIMSAL_URETIM = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_TARIMSAL_URETIM"],
                                        GRP_SANAYI_URETIM = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_SANAYI_URETIM"],
                                        GRP_HIZMET_URETIM = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_HIZMET_URETIM"],
                                        GRP_INSAAT_URETIM = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_INSAAT_URETIM"],
                                        CDD = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"CDD"],
                                        HDD = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"HDD"],
                                        `GDP_TARIMSAL_URETIM_%` = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_TARIMSAL_URETIM_%"],
                                        `GDP_SANAYI_URETIM_%` = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_SANAYI_URETIM_%"],
                                        `GDP_HIZMET_URETIM_%` = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_HIZMET_URETIM_%"],
                                        `GDP_INSAAT_URETIM_%` = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_INSAAT_URETIM_%"],
                                        `GRP_TARIMSAL_URETIM_%` = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_TARIMSAL_URETIM_%"],
                                        `GRP_SANAYI_URETIM_%` = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_SANAYI_URETIM_%"],
                                        `GRP_HIZMET_URETIM_%` = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_HIZMET_URETIM_%"],
                                        `GRP_INSAAT_URETIM_%` = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_INSAAT_URETIM_%"],
                                        KKO = d_minimum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"KKO"]))

# Clean KKO values
senaryolar_minimum$KKO <- as.numeric(senaryolar_minimum$KKO)
senaryolar_minimum$KKO[is.na(senaryolar_minimum$KKO) | is.nan(senaryolar_minimum$KKO)] <- 0


senaryolar_dusuk <- as.data.frame(cbind(YIL = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"YIL"],
                                          GDP_BUYUME_ORANI = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_BUYUME_ORANI"],
                                          ILCE_NUFUS = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"ILCE_NUFUS"],
                                          GDP = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP"],
                                          GDP_TARIMSAL_URETIM = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_TARIMSAL_URETIM"],
                                          GDP_SANAYI_URETIM = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_SANAYI_URETIM"],
                                          GDP_HIZMET_URETIM = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_HIZMET_URETIM"],
                                          GDP_INSAAT_URETIM = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_INSAAT_URETIM"],
                                          GRP = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP"],
                                          GRP_TARIMSAL_URETIM = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_TARIMSAL_URETIM"],
                                          GRP_SANAYI_URETIM = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_SANAYI_URETIM"],
                                          GRP_HIZMET_URETIM = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_HIZMET_URETIM"],
                                          GRP_INSAAT_URETIM = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_INSAAT_URETIM"],
                                          CDD = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"CDD"],
                                          HDD = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"HDD"],
                                          `GDP_TARIMSAL_URETIM_%` = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_TARIMSAL_URETIM_%"],
                                          `GDP_SANAYI_URETIM_%` = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_SANAYI_URETIM_%"],
                                          `GDP_HIZMET_URETIM_%` = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_HIZMET_URETIM_%"],
                                          `GDP_INSAAT_URETIM_%` = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_INSAAT_URETIM_%"],
                                          `GRP_TARIMSAL_URETIM_%` = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_TARIMSAL_URETIM_%"],
                                          `GRP_SANAYI_URETIM_%` = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_SANAYI_URETIM_%"],
                                          `GRP_HIZMET_URETIM_%` = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_HIZMET_URETIM_%"],
                                          `GRP_INSAAT_URETIM_%` = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_INSAAT_URETIM_%"],
                                          KKO = d_dusuk[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"KKO"]))


senaryolar_dusuk$KKO <- as.numeric(senaryolar_dusuk$KKO)
senaryolar_dusuk$KKO[is.na(senaryolar_dusuk$KKO) | is.nan(senaryolar_dusuk$KKO)] <- 0


senaryolar_baz <- as.data.frame(cbind(YIL = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"YIL"],
                                        GDP_BUYUME_ORANI = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_BUYUME_ORANI"],
                                        ILCE_NUFUS = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"ILCE_NUFUS"],
                                        GDP = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP"],
                                        GDP_TARIMSAL_URETIM = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_TARIMSAL_URETIM"],
                                        GDP_SANAYI_URETIM = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_SANAYI_URETIM"],
                                        GDP_HIZMET_URETIM = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_HIZMET_URETIM"],
                                        GDP_INSAAT_URETIM = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_INSAAT_URETIM"],
                                        GRP = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP"],
                                        GRP_TARIMSAL_URETIM = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_TARIMSAL_URETIM"],
                                        GRP_SANAYI_URETIM = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_SANAYI_URETIM"],
                                        GRP_HIZMET_URETIM = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_HIZMET_URETIM"],
                                        GRP_INSAAT_URETIM = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_INSAAT_URETIM"],
                                        CDD = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"CDD"],
                                        HDD = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"HDD"],
                                        `GDP_TARIMSAL_URETIM_%` = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_TARIMSAL_URETIM_%"],
                                        `GDP_SANAYI_URETIM_%` = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_SANAYI_URETIM_%"],
                                        `GDP_HIZMET_URETIM_%` = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_HIZMET_URETIM_%"],
                                        `GDP_INSAAT_URETIM_%` = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_INSAAT_URETIM_%"],
                                        `GRP_TARIMSAL_URETIM_%` = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_TARIMSAL_URETIM_%"],
                                        `GRP_SANAYI_URETIM_%` = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_SANAYI_URETIM_%"],
                                        `GRP_HIZMET_URETIM_%` = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_HIZMET_URETIM_%"],
                                        `GRP_INSAAT_URETIM_%` = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_INSAAT_URETIM_%"],
                                        KKO = d[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"KKO"]))


senaryolar_baz$KKO <- as.numeric(senaryolar_baz$KKO)
senaryolar_baz$KKO[is.na(senaryolar_baz$KKO) | is.nan(senaryolar_baz$KKO)] <- 0



senaryolar_yuksek <- as.data.frame(cbind(YIL = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"YIL"],
                                        GDP_BUYUME_ORANI = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_BUYUME_ORANI"],
                                        ILCE_NUFUS = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"ILCE_NUFUS"],
                                        GDP = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP"],
                                        GDP_TARIMSAL_URETIM = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_TARIMSAL_URETIM"],
                                        GDP_SANAYI_URETIM = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_SANAYI_URETIM"],
                                        GDP_HIZMET_URETIM = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_HIZMET_URETIM"],
                                        GDP_INSAAT_URETIM = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_INSAAT_URETIM"],
                                        GRP = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP"],
                                        GRP_TARIMSAL_URETIM = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_TARIMSAL_URETIM"],
                                        GRP_SANAYI_URETIM = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_SANAYI_URETIM"],
                                        GRP_HIZMET_URETIM = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_HIZMET_URETIM"],
                                        GRP_INSAAT_URETIM = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_INSAAT_URETIM"],
                                        CDD = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"CDD"],
                                        HDD = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"HDD"],
                                        `GDP_TARIMSAL_URETIM_%` = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_TARIMSAL_URETIM_%"],
                                        `GDP_SANAYI_URETIM_%` = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_SANAYI_URETIM_%"],
                                        `GDP_HIZMET_URETIM_%` = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_HIZMET_URETIM_%"],
                                        `GDP_INSAAT_URETIM_%` = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_INSAAT_URETIM_%"],
                                        `GRP_TARIMSAL_URETIM_%` = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_TARIMSAL_URETIM_%"],
                                        `GRP_SANAYI_URETIM_%` = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_SANAYI_URETIM_%"],
                                        `GRP_HIZMET_URETIM_%` = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_HIZMET_URETIM_%"],
                                        `GRP_INSAAT_URETIM_%` = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_INSAAT_URETIM_%"],
                                        KKO = d_yuksek[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"KKO"]))


senaryolar_yuksek$KKO <- as.numeric(senaryolar_yuksek$KKO)
senaryolar_yuksek$KKO[is.na(senaryolar_yuksek$KKO) | is.nan(senaryolar_yuksek$KKO)] <- 0



senaryolar_maximum <- as.data.frame(cbind(YIL = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"YIL"],
                                         GDP_BUYUME_ORANI = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_BUYUME_ORANI"],
                                         ILCE_NUFUS = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"ILCE_NUFUS"],
                                         GDP = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP"],
                                         GDP_TARIMSAL_URETIM = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_TARIMSAL_URETIM"],
                                         GDP_SANAYI_URETIM = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_SANAYI_URETIM"],
                                         GDP_HIZMET_URETIM = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_HIZMET_URETIM"],
                                         GDP_INSAAT_URETIM = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_INSAAT_URETIM"],
                                         GRP = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP"],
                                         GRP_TARIMSAL_URETIM = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_TARIMSAL_URETIM"],
                                         GRP_SANAYI_URETIM = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_SANAYI_URETIM"],
                                         GRP_HIZMET_URETIM = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_HIZMET_URETIM"],
                                         GRP_INSAAT_URETIM = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_INSAAT_URETIM"],
                                         CDD = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"CDD"],
                                         HDD = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"HDD"],
                                         `GDP_TARIMSAL_URETIM_%` = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_TARIMSAL_URETIM_%"],
                                         `GDP_SANAYI_URETIM_%` = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_SANAYI_URETIM_%"],
                                         `GDP_HIZMET_URETIM_%` = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_HIZMET_URETIM_%"],
                                         `GDP_INSAAT_URETIM_%` = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GDP_INSAAT_URETIM_%"],
                                         `GRP_TARIMSAL_URETIM_%` = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_TARIMSAL_URETIM_%"],
                                         `GRP_SANAYI_URETIM_%` = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_SANAYI_URETIM_%"],
                                         `GRP_HIZMET_URETIM_%` = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_HIZMET_URETIM_%"],
                                         `GRP_INSAAT_URETIM_%` = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"GRP_INSAAT_URETIM_%"],
                                         KKO = d_maximum[(last_known_year_nufus_index+1):(last_known_year_nufus_index+h+1),"KKO"]))


senaryolar_maximum$KKO <- as.numeric(senaryolar_maximum$KKO)
senaryolar_maximum$KKO[is.na(senaryolar_maximum$KKO) | is.nan(senaryolar_maximum$KKO)] <- 0


degerler_last_known_year_nufus <- as.data.frame(cbind(GRP = d[(last_known_year_nufus_index),"GRP"],
                                                      GRP_TARIMSAL_URETIM = d[(last_known_year_nufus_index),"GRP_TARIMSAL_URETIM"],
                                                      GRP_SANAYI_URETIM = d[(last_known_year_nufus_index),"GRP_SANAYI_URETIM"],
                                                      GRP_HIZMET_URETIM = d[(last_known_year_nufus_index),"GRP_HIZMET_URETIM"],
                                                      GRP_INSAAT_URETIM = d[(last_known_year_nufus_index),"GRP_INSAAT_URETIM"],
                                                      `GRP_TARIMSAL_URETIM_%` = d[(last_known_year_nufus_index),"GRP_TARIMSAL_URETIM_%"],
                                                      `GRP_SANAYI_URETIM_%` = d[(last_known_year_nufus_index),"GRP_SANAYI_URETIM_%"],
                                                      `GRP_HIZMET_URETIM_%` = d[(last_known_year_nufus_index),"GRP_HIZMET_URETIM_%"],
                                                      `GRP_INSAAT_URETIM_%` = d[(last_known_year_nufus_index),"GRP_INSAAT_URETIM_%"],
                                                      GDP = d[(last_known_year_nufus_index),"GDP"],
                                                      GDP_TARIMSAL_URETIM = d[(last_known_year_nufus_index),"GDP_TARIMSAL_URETIM"],
                                                      GDP_SANAYI_URETIM = d[(last_known_year_nufus_index),"GDP_SANAYI_URETIM"],
                                                      GDP_HIZMET_URETIM = d[(last_known_year_nufus_index),"GDP_HIZMET_URETIM"],
                                                      GDP_INSAAT_URETIM = d[(last_known_year_nufus_index),"GDP_INSAAT_URETIM"],
                                                      `GDP_TARIMSAL_URETIM_%` = d[(last_known_year_nufus_index),"GDP_TARIMSAL_URETIM_%"],
                                                      `GDP_SANAYI_URETIM_%` = d[(last_known_year_nufus_index),"GDP_SANAYI_URETIM_%"],
                                                      `GDP_HIZMET_URETIM_%` = d[(last_known_year_nufus_index),"GDP_HIZMET_URETIM_%"],
                                                      `GDP_INSAAT_URETIM_%` = d[(last_known_year_nufus_index),"GDP_INSAAT_URETIM_%"]))



# create a workbook object on R given the original data file
workbook <- loadWorkbook(excel_file_path)

# Delete contents of all cells in sheets 2, 3, 4, 5, and 6 starting from row 2
for (sheet in 2:6) {
  deleteData(wb = workbook, 
             sheet = sheet, 
             cols = 1:30,  # Cover up to 30 columns (adjust if needed)
             rows = 2:1000, # Cover rows starting from 2
             gridExpand = TRUE)  # Ensure full grid is cleared
}

# create style to be used for "bottom up 1", "bottom up 2" and "top down 1" models
my_style <- createStyle(fontName = "Calibri",
                        fontSize = 11,
                        valign = "center",
                        halign = "center",
                        numFmt = "#,##0")

writeData(wb = workbook,
          sheet = 2,
          x = senaryolar_minimum,
          startCol = 1,
          startRow = 2,
          colNames = FALSE,
          rowNames = FALSE)

writeData(wb = workbook,
          sheet = 3,
          x = senaryolar_dusuk,
          startCol = 1,
          startRow = 2,
          colNames = FALSE,
          rowNames = FALSE)

writeData(wb = workbook,
          sheet = 4,
          x = senaryolar_baz,
          startCol = 1,
          startRow = 2,
          colNames = FALSE,
          rowNames = FALSE)

writeData(wb = workbook,
          sheet = 5,
          x = senaryolar_yuksek,
          startCol = 1,
          startRow = 2,
          colNames = FALSE,
          rowNames = FALSE)

writeData(wb = workbook,
          sheet = 6,
          x = senaryolar_maximum,
          startCol = 1,
          startRow = 2,
          colNames = FALSE,
          rowNames = FALSE)

# Replace NA values with empty strings ("")
degerler_last_known_year_nufus <- as.data.frame(lapply(degerler_last_known_year_nufus, function(x) {
  ifelse(is.na(x), "", x)
}))


# Write the modified data frame to the Excel workbook
writeData(
      wb = workbook,
      sheet = 1,
      x = degerler_last_known_year_nufus,
      startCol = 24,
      startRow = last_known_year_nufus_index + 1,
      colNames = FALSE,
      rowNames = FALSE
)


# save the changes and close the workbook
saveWorkbook(workbook,
             file = excel_file_path,
             overwrite = TRUE)


