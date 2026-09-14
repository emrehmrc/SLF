
##### kullanılan paketler #####
packages <- c("Rcpp", "zip", "readr", "tidyverse", "readxl", "magrittr", "ggplot2", "gridExtra", "stringr", "car", "openxlsx", "jsonlite",
              "scales", "caret", "lattice", "MASS", "dplyr", "forecast", "this.path", "Metrics", "boot", "glmnet", "zoo", "elasticnet","ggthemes")

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

##### Config dosyasını oku #####
config <- jsonlite::fromJSON(args[1]) 

user_root_path <- Sys.getenv("USERPROFILE")

# çalışma klasörünü current klasöre ayarla
setwd(this.dir())

# Model parametrelerinin çıktılarını veren klasör varsa onu sil ve yeni klasör oluştur
unlink(paste0(user_root_path,"/",
              config$Ana_Klasör_Yolu, "/",
                     config$İl, "/",
                     config$İlçe, "/",
                     config$ELF$SONUÇLAR_klasör, "/",
              "Model Çıktıları"), recursive = TRUE)

dir.create(paste0(user_root_path,"/",
                  config$Ana_Klasör_Yolu, "/",
                  config$İl, "/",
                  config$İlçe, "/",
                  config$ELF$SONUÇLAR_klasör, "/",
                  "Model Çıktıları"))

# Grafik çıktılarını veren klasör varsa onu sil ve yeni klasör oluştur
unlink(paste0(user_root_path,"/",
              config$Ana_Klasör_Yolu, "/",
              config$İl, "/",
              config$İlçe, "/",
              config$ELF$SONUÇLAR_klasör, "/",
              "Grafik Çıktıları"), recursive = TRUE)

dir.create(paste0(user_root_path,"/",
                  config$Ana_Klasör_Yolu, "/",
                  config$İl, "/",
                  config$İlçe, "/",
                  config$ELF$SONUÇLAR_klasör, "/",
                  "Grafik Çıktıları"))

# uyarıları süpres et
options(warn = -1)


excel_input_file_path <- paste0(user_root_path,"/",
                                config$Ana_Klasör_Yolu, "/",
                          config$İl, "/",
                          config$İlçe, "/",
                          config$ELF$INPUT_FILE)

excel_template_path <- paste0(user_root_path,"/",
                              config$Ana_Klasör_Yolu, "/",
                                config$İl, "/",
                                config$İlçe, "/",
                                config$ELF$ŞABLON)


##### Ana girdi dosyasını yükle #####
d <- read_excel(excel_input_file_path,
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



# ---------------------------------------------------------------------------------------- #

##### EXTRACT NECESSARY DATA #####

# starting index of the Mesken_faturalanan - all other consumption values
index <- which(!is.na(d$MESKEN_FATURALANAN))[1]



print("Bölgeye ait tüketim ve abone sayıları girdi verileri okunuyor...")

Sys.sleep(4)


# full dataset related with bolge's billed consumption since 2013
bolge_full_2013 <- as.data.frame(cbind(Mesken_tuketim = data_preparation(filled_cells(d, 12))$faturalanan,
                                       Sanayi_tuketim = data_preparation(filled_cells(d, 13))$faturalanan,
                                       Ticarethane_tuketim = data_preparation(filled_cells(d, 14))$faturalanan,
                                       Sulama_tuketim = data_preparation(filled_cells(d, 15))$faturalanan,
                                       Aydınlatma_tuketim = data_preparation(filled_cells(d, 16))$faturalanan,
                                       GRP = d[index:(index+nrow_dataset_tuketim-1),24],
                                       GRP_lag1 = d[(index-1):(index-1+nrow_dataset_tuketim-1),24],
                                       ILCE_NUFUS = d[index:(index+nrow_dataset_tuketim-1),3],
                                       Sanayi_üretimi = d[index:(index+nrow_dataset_tuketim-1),26],
                                       Tarım_üretimi = d[index:(index+nrow_dataset_tuketim-1),25],
                                       Hizmet_üretimi = d[index:(index+nrow_dataset_tuketim-1),27],
                                       Insaat_üretimi = d[index:(index+nrow_dataset_tuketim-1),28],
                                       HDD = d[index:(index+nrow_dataset_tuketim-1),43],
                                       CDD = d[index:(index+nrow_dataset_tuketim-1),42],
                                       Mesken_abone = data_preparation(filled_cells(d, 18))$faturalanan %>% round(0),
                                       Sanayi_abone = data_preparation(filled_cells(d, 19))$faturalanan %>% round(0),
                                       Ticarethane_abone = data_preparation(filled_cells(d, 20))$faturalanan %>% round(0),
                                       Sulama_abone = data_preparation(filled_cells(d, 21))$faturalanan %>% round(0),
                                       Aydınlatma_abone = data_preparation(filled_cells(d, 22))$faturalanan %>% round(0)))

# full dataset related with bolge's billed consumption since 2014
bolge_full_2014 <- as.data.frame(cbind(Mesken_tuketim = data_preparation(filled_cells(d, 12))$faturalanan[-1],
                                       Mesken_tuketim_lag1 = data_preparation(filled_cells(d, 12))$faturalanan[-nrow_dataset_tuketim],
                                       Sanayi_tuketim = data_preparation(filled_cells(d, 13))$faturalanan[-1],
                                       Sanayi_tuketim_lag1 = data_preparation(filled_cells(d, 13))$faturalanan[-nrow_dataset_tuketim],
                                       Ticarethane_tuketim = data_preparation(filled_cells(d, 14))$faturalanan[-1],
                                       Ticarethane_tuketim_lag1 = data_preparation(filled_cells(d, 14))$faturalanan[-nrow_dataset_tuketim],
                                       Sulama_tuketim = data_preparation(filled_cells(d, 15))$faturalanan[-1],
                                       Sulama_tuketim_lag1 = data_preparation(filled_cells(d, 15))$faturalanan[-nrow_dataset_tuketim],
                                       Aydınlatma_tuketim = data_preparation(filled_cells(d, 16))$faturalanan[-1],
                                       Aydınlatma_tuketim_lag1 = data_preparation(filled_cells(d, 16))$faturalanan[-nrow_dataset_tuketim],
                                       GRP = d[(index+1):(index+nrow_dataset_tuketim-1),25][-nrow_dataset_tuketim],
                                       GRP_lag1 = d[(index):(index+nrow_dataset_tuketim-1),25][-nrow_dataset_tuketim],
                                       ILCE_NUFUS = d[(index+1):(index+nrow_dataset_tuketim-1),3][-nrow_dataset_tuketim],
                                       Sanayi_üretimi = d[(index+1):(index+nrow_dataset_tuketim-1),26][-nrow_dataset_tuketim],
                                       Tarım_üretimi = d[(index+1):(index+nrow_dataset_tuketim-1),25][-nrow_dataset_tuketim],
                                       Hizmet_üretimi = d[(index+1):(index+nrow_dataset_tuketim-1),27][-nrow_dataset_tuketim],
                                       Insaat_üretimi = d[(index+1):(index+nrow_dataset_tuketim-1),28][-nrow_dataset_tuketim],
                                       HDD = d[(index+1):(index+nrow_dataset_tuketim-1),43][-nrow_dataset_tuketim],
                                       CDD = d[(index+1):(index+nrow_dataset_tuketim-1),42][-nrow_dataset_tuketim],
                                       Mesken_abone = data_preparation(filled_cells(d, 18))$faturalanan[-1],
                                       Mesken_abone_lag1 = data_preparation(filled_cells(d, 18))$faturalanan[-nrow_dataset_tuketim],
                                       Sanayi_abone = data_preparation(filled_cells(d, 19))$faturalanan[-1],
                                       Sanayi_abone_lag1 = data_preparation(filled_cells(d, 19))$faturalanan[-nrow_dataset_tuketim],
                                       Ticarethane_abone = data_preparation(filled_cells(d, 20))$faturalanan[-1],
                                       Ticarethane_abone_lag1 = data_preparation(filled_cells(d, 20))$faturalanan[-nrow_dataset_tuketim],
                                       Sulama_abone = data_preparation(filled_cells(d, 21))$faturalanan[-1],
                                       Sulama_abone_lag1 = data_preparation(filled_cells(d, 21))$faturalanan[-nrow_dataset_tuketim],
                                       Aydınlatma_abone = data_preparation(filled_cells(d, 22))$faturalanan[-1],
                                       Aydınlatma_abone_lag1 = data_preparation(filled_cells(d, 22))$faturalanan[-nrow_dataset_tuketim]))



# SENARYOLAR #
senaryolar_1 <- read_excel(excel_input_file_path,  
                           sheet = 2,
                           col_names = TRUE,
                           col_types = rep("numeric",24)) %>% as.data.frame()
senaryolar_2 <- read_excel(excel_input_file_path,
                           sheet = 3,
                           col_names = TRUE,
                           col_types = rep("numeric",24)) %>% as.data.frame()

senaryolar_3 <- read_excel(excel_input_file_path,
                           sheet = 4,
                           col_names = TRUE,
                           col_types = rep("numeric",24)) %>% as.data.frame()

senaryolar_4 <- read_excel(excel_input_file_path,
                           sheet = 5,
                           col_names = TRUE,
                           col_types = rep("numeric",24)) %>% as.data.frame()
senaryolar_5 <- read_excel(excel_input_file_path,
                           sheet = 6,
                           col_names = TRUE,
                           col_types = rep("numeric",24)) %>% as.data.frame()

print("Senaryolar okunuyor...")

Sys.sleep(3)



######------------------ NEW DATA ------------------######

# new values of the predictor features of bolge
bolge_full_new_minimum <- as.data.frame(cbind(
  GRP = senaryolar_1[1:(h), "GRP"],
  GRP_lag1 = append(tail(filled_cells(d, 24), h)[h-1], 
                    senaryolar_1[1:(h-1), "GRP"]),
  ILCE_NUFUS = senaryolar_1[1:(h), "ILCE_NUFUS"],
  Sanayi_üretimi = senaryolar_1[1:(h), "GRP_SANAYI_URETIM"],
  Tarım_üretimi = senaryolar_1[1:(h), "GRP_TARIMSAL_URETIM"],
  Insaat_üretimi = senaryolar_1[1:(h), "GRP_INSAAT_URETIM"],
  Hizmet_üretimi = senaryolar_1[1:(h), "GRP_HIZMET_URETIM"],
  HDD = senaryolar_1[1:(h), "HDD"],
  CDD = senaryolar_1[1:(h), "CDD"],
  rbind(Mesken_tuketim_lag1 = data_preparation(filled_cells(d, 12))$faturalanan[nrow_dataset_tuketim],
        data.frame(Mesken_tuketim_lag1 = rep(NA, h-1))),
  rbind(Sanayi_tuketim_lag1 = data_preparation(filled_cells(d, 13))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sanayi_tuketim_lag1 = rep(NA, h-1))),
  rbind(Ticarethane_tuketim_lag1 = data_preparation(filled_cells(d, 14))$faturalanan[nrow_dataset_tuketim],
        data.frame(Ticarethane_tuketim_lag1 = rep(NA, h-1))),
  rbind(Sulama_tuketim_lag1 = data_preparation(filled_cells(d, 15))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sulama_tuketim_lag1 = rep(NA, h-1))),
  rbind(Aydınlatma_tuketim_lag1 = data_preparation(filled_cells(d, 16))$faturalanan[nrow_dataset_tuketim],
        data.frame(Aydınlatma_tuketim_lag1 = rep(NA, h-1))),
  rbind(Mesken_abone_lag1 = data_preparation(filled_cells(d, 18))$faturalanan[nrow_dataset_tuketim],
        data.frame(Mesken_abone_lag1 = rep(NA, h-1))),
  rbind(Sanayi_abone_lag1 = data_preparation(filled_cells(d, 19))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sanayi_abone_lag1 = rep(NA, h-1))),
  rbind(Ticarethane_abone_lag1 = data_preparation(filled_cells(d, 20))$faturalanan[nrow_dataset_tuketim],
        data.frame(Ticarethane_abone_lag1 = rep(NA, h-1))),
  rbind(Sulama_abone_lag1 = data_preparation(filled_cells(d, 21))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sulama_abone_lag1 = rep(NA, h-1))),
  rbind(Aydınlatma_abone_lag1 = data_preparation(filled_cells(d, 22))$faturalanan[nrow_dataset_tuketim],
        data.frame(Aydınlatma_abone_lag1 = rep(NA, h-1)))))


# new values of the predictor features of bolge
bolge_full_new_dusuk <- as.data.frame(cbind(
  GRP = senaryolar_2[1:(h), "GRP"],
  GRP_lag1 = append(tail(filled_cells(d, 24), h)[h-1], 
                    senaryolar_2[1:(h-1), "GRP"]),
  ILCE_NUFUS = senaryolar_2[1:(h), "ILCE_NUFUS"],
  Sanayi_üretimi = senaryolar_2[1:(h), "GRP_SANAYI_URETIM"],
  Tarım_üretimi = senaryolar_2[1:(h), "GRP_TARIMSAL_URETIM"],
  Insaat_üretimi = senaryolar_2[1:(h), "GRP_INSAAT_URETIM"],
  Hizmet_üretimi = senaryolar_2[1:(h), "GRP_HIZMET_URETIM"],
  HDD = senaryolar_2[1:(h), "HDD"],
  CDD = senaryolar_2[1:(h), "CDD"],
  rbind(Mesken_tuketim_lag1 = data_preparation(filled_cells(d, 12))$faturalanan[nrow_dataset_tuketim],
        data.frame(Mesken_tuketim_lag1 = rep(NA, h-1))),
  rbind(Sanayi_tuketim_lag1 = data_preparation(filled_cells(d, 13))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sanayi_tuketim_lag1 = rep(NA, h-1))),
  rbind(Ticarethane_tuketim_lag1 = data_preparation(filled_cells(d, 14))$faturalanan[nrow_dataset_tuketim],
        data.frame(Ticarethane_tuketim_lag1 = rep(NA, h-1))),
  rbind(Sulama_tuketim_lag1 = data_preparation(filled_cells(d, 15))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sulama_tuketim_lag1 = rep(NA, h-1))),
  rbind(Aydınlatma_tuketim_lag1 = data_preparation(filled_cells(d, 16))$faturalanan[nrow_dataset_tuketim],
        data.frame(Aydınlatma_tuketim_lag1 = rep(NA, h-1))),
  rbind(Mesken_abone_lag1 = data_preparation(filled_cells(d, 18))$faturalanan[nrow_dataset_tuketim],
        data.frame(Mesken_abone_lag1 = rep(NA, h-1))),
  rbind(Sanayi_abone_lag1 = data_preparation(filled_cells(d, 19))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sanayi_abone_lag1 = rep(NA, h-1))),
  rbind(Ticarethane_abone_lag1 = data_preparation(filled_cells(d, 20))$faturalanan[nrow_dataset_tuketim],
        data.frame(Ticarethane_abone_lag1 = rep(NA, h-1))),
  rbind(Sulama_abone_lag1 = data_preparation(filled_cells(d, 21))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sulama_abone_lag1 = rep(NA, h-1))),
  rbind(Aydınlatma_abone_lag1 = data_preparation(filled_cells(d, 22))$faturalanan[nrow_dataset_tuketim],
        data.frame(Aydınlatma_abone_lag1 = rep(NA, h-1)))))


# new values of the predictor features of bolge
bolge_full_new_baz <- as.data.frame(cbind(
  GRP = senaryolar_3[1:(h), "GRP"],
  GRP_lag1 = append(tail(filled_cells(d, 24), h)[h-1], 
                    senaryolar_3[1:(h-1), "GRP"]),
  ILCE_NUFUS = senaryolar_3[1:(h), "ILCE_NUFUS"],
  Sanayi_üretimi = senaryolar_3[1:(h), "GRP_SANAYI_URETIM"],
  Tarım_üretimi = senaryolar_3[1:(h), "GRP_TARIMSAL_URETIM"],
  Insaat_üretimi = senaryolar_3[1:(h), "GRP_INSAAT_URETIM"],
  Hizmet_üretimi = senaryolar_3[1:(h), "GRP_HIZMET_URETIM"],
  HDD = senaryolar_3[1:(h), "HDD"],
  CDD = senaryolar_3[1:(h), "CDD"],
  rbind(Mesken_tuketim_lag1 = data_preparation(filled_cells(d, 12))$faturalanan[nrow_dataset_tuketim],
        data.frame(Mesken_tuketim_lag1 = rep(NA, h-1))),
  rbind(Sanayi_tuketim_lag1 = data_preparation(filled_cells(d, 13))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sanayi_tuketim_lag1 = rep(NA, h-1))),
  rbind(Ticarethane_tuketim_lag1 = data_preparation(filled_cells(d, 14))$faturalanan[nrow_dataset_tuketim],
        data.frame(Ticarethane_tuketim_lag1 = rep(NA, h-1))),
  rbind(Sulama_tuketim_lag1 = data_preparation(filled_cells(d, 15))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sulama_tuketim_lag1 = rep(NA, h-1))),
  rbind(Aydınlatma_tuketim_lag1 = data_preparation(filled_cells(d, 16))$faturalanan[nrow_dataset_tuketim],
        data.frame(Aydınlatma_tuketim_lag1 = rep(NA, h-1))),
  rbind(Mesken_abone_lag1 = data_preparation(filled_cells(d, 18))$faturalanan[nrow_dataset_tuketim],
        data.frame(Mesken_abone_lag1 = rep(NA, h-1))),
  rbind(Sanayi_abone_lag1 = data_preparation(filled_cells(d, 19))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sanayi_abone_lag1 = rep(NA, h-1))),
  rbind(Ticarethane_abone_lag1 = data_preparation(filled_cells(d, 20))$faturalanan[nrow_dataset_tuketim],
        data.frame(Ticarethane_abone_lag1 = rep(NA, h-1))),
  rbind(Sulama_abone_lag1 = data_preparation(filled_cells(d, 21))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sulama_abone_lag1 = rep(NA, h-1))),
  rbind(Aydınlatma_abone_lag1 = data_preparation(filled_cells(d, 22))$faturalanan[nrow_dataset_tuketim],
        data.frame(Aydınlatma_abone_lag1 = rep(NA, h-1))),
  rbind(summer_demand_lag1 = data_preparation(filled_cells(d, 24))$faturalanan[nrow_dataset_tuketim],
        data.frame(summer_demand_lag1 = rep(NA, h-1)))))


# new values of the predictor features of bolge
bolge_full_new_yuksek <- as.data.frame(cbind(
  GRP = senaryolar_4[1:(h), "GRP"],
  GRP_lag1 = append(tail(filled_cells(d, 24), h)[h-1], 
                    senaryolar_4[1:(h-1), "GRP"]),
  ILCE_NUFUS = senaryolar_4[1:(h), "ILCE_NUFUS"],
  Sanayi_üretimi = senaryolar_4[1:(h), "GRP_SANAYI_URETIM"],
  Tarım_üretimi = senaryolar_4[1:(h), "GRP_TARIMSAL_URETIM"],
  Insaat_üretimi = senaryolar_4[1:(h), "GRP_INSAAT_URETIM"],
  Hizmet_üretimi = senaryolar_4[1:(h), "GRP_HIZMET_URETIM"],
  HDD = senaryolar_4[1:(h), "HDD"],
  CDD = senaryolar_4[1:(h), "CDD"],
  rbind(Mesken_tuketim_lag1 = data_preparation(filled_cells(d, 12))$faturalanan[nrow_dataset_tuketim],
        data.frame(Mesken_tuketim_lag1 = rep(NA, h-1))),
  rbind(Sanayi_tuketim_lag1 = data_preparation(filled_cells(d, 13))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sanayi_tuketim_lag1 = rep(NA, h-1))),
  rbind(Ticarethane_tuketim_lag1 = data_preparation(filled_cells(d, 14))$faturalanan[nrow_dataset_tuketim],
        data.frame(Ticarethane_tuketim_lag1 = rep(NA, h-1))),
  rbind(Sulama_tuketim_lag1 = data_preparation(filled_cells(d, 15))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sulama_tuketim_lag1 = rep(NA, h-1))),
  rbind(Aydınlatma_tuketim_lag1 = data_preparation(filled_cells(d, 16))$faturalanan[nrow_dataset_tuketim],
        data.frame(Aydınlatma_tuketim_lag1 = rep(NA, h-1))),
  rbind(Mesken_abone_lag1 = data_preparation(filled_cells(d, 18))$faturalanan[nrow_dataset_tuketim],
        data.frame(Mesken_abone_lag1 = rep(NA, h-1))),
  rbind(Sanayi_abone_lag1 = data_preparation(filled_cells(d, 19))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sanayi_abone_lag1 = rep(NA, h-1))),
  rbind(Ticarethane_abone_lag1 = data_preparation(filled_cells(d, 20))$faturalanan[nrow_dataset_tuketim],
        data.frame(Ticarethane_abone_lag1 = rep(NA, h-1))),
  rbind(Sulama_abone_lag1 = data_preparation(filled_cells(d, 21))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sulama_abone_lag1 = rep(NA, h-1))),
  rbind(Aydınlatma_abone_lag1 = data_preparation(filled_cells(d, 22))$faturalanan[nrow_dataset_tuketim],
        data.frame(Aydınlatma_abone_lag1 = rep(NA, h-1)))))


# new values of the predictor features of bolge
bolge_full_new_maksimum <- as.data.frame(cbind(
  GRP = senaryolar_5[1:(h), "GRP"],
  GRP_lag1 = append(tail(filled_cells(d, 24), h)[h-1], 
                    senaryolar_5[1:(h-1), "GRP"]),
  ILCE_NUFUS = senaryolar_5[1:(h), "ILCE_NUFUS"],
  Sanayi_üretimi = senaryolar_5[1:(h), "GRP_SANAYI_URETIM"],
  Tarım_üretimi = senaryolar_5[1:(h), "GRP_TARIMSAL_URETIM"],
  Insaat_üretimi = senaryolar_5[1:(h), "GRP_INSAAT_URETIM"],
  Hizmet_üretimi = senaryolar_5[1:(h), "GRP_HIZMET_URETIM"],
  HDD = senaryolar_5[1:(h), "HDD"],
  CDD = senaryolar_5[1:(h), "CDD"],
  rbind(Mesken_tuketim_lag1 = data_preparation(filled_cells(d, 12))$faturalanan[nrow_dataset_tuketim],
        data.frame(Mesken_tuketim_lag1 = rep(NA, h-1))),
  rbind(Sanayi_tuketim_lag1 = data_preparation(filled_cells(d, 13))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sanayi_tuketim_lag1 = rep(NA, h-1))),
  rbind(Ticarethane_tuketim_lag1 = data_preparation(filled_cells(d, 14))$faturalanan[nrow_dataset_tuketim],
        data.frame(Ticarethane_tuketim_lag1 = rep(NA, h-1))),
  rbind(Sulama_tuketim_lag1 = data_preparation(filled_cells(d, 15))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sulama_tuketim_lag1 = rep(NA, h-1))),
  rbind(Aydınlatma_tuketim_lag1 = data_preparation(filled_cells(d, 16))$faturalanan[nrow_dataset_tuketim],
        data.frame(Aydınlatma_tuketim_lag1 = rep(NA, h-1))),
  rbind(Mesken_abone_lag1 = data_preparation(filled_cells(d, 18))$faturalanan[nrow_dataset_tuketim],
        data.frame(Mesken_abone_lag1 = rep(NA, h-1))),
  rbind(Sanayi_abone_lag1 = data_preparation(filled_cells(d, 19))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sanayi_abone_lag1 = rep(NA, h-1))),
  rbind(Ticarethane_abone_lag1 = data_preparation(filled_cells(d, 20))$faturalanan[nrow_dataset_tuketim],
        data.frame(Ticarethane_abone_lag1 = rep(NA, h-1))),
  rbind(Sulama_abone_lag1 = data_preparation(filled_cells(d, 21))$faturalanan[nrow_dataset_tuketim],
        data.frame(Sulama_abone_lag1 = rep(NA, h-1))),
  rbind(Aydınlatma_abone_lag1 = data_preparation(filled_cells(d, 22))$faturalanan[nrow_dataset_tuketim],
        data.frame(Aydınlatma_abone_lag1 = rep(NA, h-1)))))

print("Ufuk yıllarına dair tahminlerde kullanılacak yeni veriler okunuyor...")

Sys.sleep(3)



###################################    MODELS    ###################################

### Linear Regression Models ###:

# function to use for stepwise linear regression models with several different predictors
# first dataset is the one that starts from 2013, and the latter is that starts from 2014
best_regression_model <- function(d1, d2, target, d1_name, target_name){


  if (nrow_dataset_tuketim >= 10){

    # null models
    trials_1 <- lm(as.formula(paste0(target, "~", "1")),
                   data = d1)
    trials_2 <- lm(as.formula(paste0(target, "~", "1")),
                   data = d2)

    # use "Sanayi_üretimi" as predictor for industry consumption, otherwise don't use it
    if(target == "Sanayi_tuketim"){

      # stepwise feature selection for the best model with BIC - goodness of fit - for ı
      best_model_stepwise_1 <- stepAIC(trials_1,
                                       scope = list(upper = as.formula(paste0("~ ILCE_NUFUS + HDD + CDD + Sanayi_üretimi")),
                                                    lower = ~ 1),
                                       trace = FALSE,
                                       k = log(nrow(d1)))

      # stepwise feature selection for the best model with BIC - goodness of fit - for d2
      best_model_stepwise_2 <- stepAIC(trials_2,
                                       scope = list(upper = as.formula(paste0("~ ILCE_NUFUS + HDD + CDD + Sanayi_üretimi + ",
                                                                              paste0(target, "_lag1"))),
                                                    lower = ~ 1),
                                       trace = FALSE,
                                       k = log(nrow(d2)))


      # while there are variables causing multicollinearity, remove them until no more collinearity exists
      while((length(best_model_stepwise_1$coefficients) > 2) && any(vif(best_model_stepwise_1) > 5)) {

        # name of the variable that is gonna be removed
        var_to_remove <- names(vif(best_model_stepwise_1)[vif(best_model_stepwise_1) > 5])[1]

        # update the model
        best_model_stepwise_1 <- best_model_stepwise_1 %>% update(as.formula(paste0("~ . - ", var_to_remove)))


      }

      # while there are variables causing multicollinearity, remove them until no more collinearity exists
      while((length(best_model_stepwise_2$coefficients) > 2) && any(vif(best_model_stepwise_2) > 5)) {

        # name of the variable that is gonna be removed
        var_to_remove <- names(vif(best_model_stepwise_2)[vif(best_model_stepwise_2) > 5])[1]

        # update the model
        best_model_stepwise_2 <- best_model_stepwise_2 %>% update(as.formula(paste0("~ . - ", var_to_remove)))


      }


    } else if (target == "Sulama_tuketim"){


      # stepwise feature selection for the best model with BIC - goodness of fit - for ı
      best_model_stepwise_1 <- stepAIC(trials_1,
                                       scope = list(upper = as.formula(paste0("~  GRP")),
                                                    lower = ~ 1),
                                       trace = FALSE,
                                       k = log(nrow(d1)))


      # stepwise feature selection for the best model with BIC - goodness of fit - for ı
      best_model_stepwise_2 <- stepAIC(trials_2,
                                       scope = list(upper = as.formula(paste0("~ GRP + ",
                                                                              paste0(target, "_lag1"))),
                                                    lower = ~ 1),
                                       trace = FALSE,
                                       k = log(nrow(d2)))

      # while there are variables causing multicollinearity, remove them until no more collinearity exists
      while((length(best_model_stepwise_1$coefficients) > 2) && any(vif(best_model_stepwise_1) > 5)) {

        # name of the variable that is gonna be removed
        var_to_remove <- names(vif(best_model_stepwise_1)[vif(best_model_stepwise_1) > 5])[1]

        # update the model
        best_model_stepwise_1 <- best_model_stepwise_1 %>% update(as.formula(paste0("~ . - ", var_to_remove)))


      }

      # while there are variables causing multicollinearity, remove them until no more collinearity exists
      while((length(best_model_stepwise_2$coefficients) > 2) && any(vif(best_model_stepwise_2) > 5)) {

        # name of the variable that is gonna be removed
        var_to_remove <- names(vif(best_model_stepwise_2)[vif(best_model_stepwise_2) > 5])[1]

        # update the model
        best_model_stepwise_2 <- best_model_stepwise_2 %>% update(as.formula(paste0("~ . - ", var_to_remove)))


      }


    } else if (target == "Ticarethane_tuketim") {

      # stepwise feature selection for the best model with BIC - goodness of fit - for ı
      best_model_stepwise_1 <- stepAIC(trials_1,
                                       scope = list(upper = as.formula(paste0("~ ILCE_NUFUS + Insaat_üretimi + Hizmet_üretimi + GRP")),
                                                    lower = ~ 1),
                                       trace = FALSE,
                                       k = log(nrow(d1)))




      # stepwise feature selection for the best model with BIC - goodness of fit - for ı
      best_model_stepwise_2 <- stepAIC(trials_2,
                                       scope = list(upper = as.formula(paste0("~  ILCE_NUFUS + HDD + CDD + Hizmet_üretimi + GRP + ",
                                                                              paste0(target, "_lag1"))),
                                                    lower = ~ 1),
                                       trace = FALSE,
                                       k = log(nrow(d2)))

      # while there are variables causing multicollinearity, remove them until no more collinearity exists
      while((length(best_model_stepwise_1$coefficients) > 2) && any(vif(best_model_stepwise_1) > 5)) {

        # name of the variable that is gonna be removed
        var_to_remove <- names(vif(best_model_stepwise_1)[vif(best_model_stepwise_1) > 5])[1]

        # update the model
        best_model_stepwise_1 <- best_model_stepwise_1 %>% update(as.formula(paste0("~ . - ", var_to_remove)))


      }

      # while there are variables causing multicollinearity, remove them until no more collinearity exists
      while((length(best_model_stepwise_2$coefficients) > 2) && any(vif(best_model_stepwise_2) > 5)) {

        # name of the variable that is gonna be removed
        var_to_remove <- names(vif(best_model_stepwise_2)[vif(best_model_stepwise_2) > 5])[1]

        # update the model
        best_model_stepwise_2 <- best_model_stepwise_2 %>% update(as.formula(paste0("~ . - ", var_to_remove)))


      }


    } else {

      # stepwise feature selection for the best model with BIC - goodness of fit - for ı
      best_model_stepwise_1 <- stepAIC(trials_1,
                                       scope = list(upper = as.formula(paste0("~  ILCE_NUFUS + GRP + Insaat_üretimi +
                                                                            Sanayi_üretimi + Hizmet_üretimi + Tarım_üretimi")),
                                                    lower = ~ 1),
                                       trace = FALSE,
                                       k = log(nrow(d1)))

      # stepwise feature selection for the best model with BIC - goodness of fit - for d2
      best_model_stepwise_2 <- stepAIC(trials_2,
                                       scope = list(upper = as.formula(paste0("~  ILCE_NUFUS + GRP + HDD + CDD + Insaat_üretimi +
                                                                            Sanayi_üretimi + Hizmet_üretimi + Tarım_üretimi + ",
                                                                              paste0(target, "_lag1"))),
                                                    lower = ~ 1),
                                       trace = FALSE,
                                       k = log(nrow(d2)))


      # while there are variables causing multicollinearity, remove them until no more collinearity exists
      while((length(best_model_stepwise_1$coefficients) > 2) && any(vif(best_model_stepwise_1) > 5)) {

        # name of the variable that is gonna be removed
        var_to_remove <- names(vif(best_model_stepwise_1)[vif(best_model_stepwise_1) > 5])[1]

        # update the model
        best_model_stepwise_1 <- best_model_stepwise_1 %>% update(as.formula(paste0("~ . - ", var_to_remove)))


      }

      # while there are variables causing multicollinearity, remove them until no more collinearity exists
      while((length(best_model_stepwise_2$coefficients) > 2) && any(vif(best_model_stepwise_2) > 5)) {

        # name of the variable that is gonna be removed
        var_to_remove <- names(vif(best_model_stepwise_2)[vif(best_model_stepwise_2) > 5])[1]

        # update the model
        best_model_stepwise_2 <- best_model_stepwise_2 %>% update(as.formula(paste0("~ . - ", var_to_remove)))


      }
    }

    # in order for cross validation to work, make sure that at least 1 predictor is selected.
    # Else, get a model with only GRP - for ı
    if(best_model_stepwise_1$rank > 1){

      # set a seed
      set.seed(1)

      # calculate the cross validation error of the chosen best_model_stepwise
      cv_model_stepwise_1 <- train(as.formula(best_model_stepwise_1$call),
                                   data = d1,
                                   method = "lm",
                                   trControl = trainControl(method = "repeatedcv",
                                                            repeats = 50,
                                                            number = 3))
    } else {

      cv_model_stepwise_1 <- lm(as.formula(paste0(target, "~", "GRP")),
                                data = d1)
    }

    # in order for cross validation to work, make sure that at least 1 predictor is selected.
    # Else, get a model with only GRP - for d2
    if(best_model_stepwise_2$rank > 1){

      # set a seed
      set.seed(2)

      # calculate the cross validation error of the chosen best_model_stepwise
      cv_model_stepwise_2 <- train(as.formula(best_model_stepwise_2$call),
                                   data = d2,
                                   method = "lm",
                                   trControl = trainControl(method = "repeatedcv",
                                                            repeats = 50,
                                                            number = 3))
    } else {

      cv_model_stepwise_2 <- lm(as.formula(paste0(target, "~", "GRP")),
                                data = d2)
    }

    ### print the resulting models to their text files
    sink(paste0(user_root_path,"/",
                config$Ana_Klasör_Yolu, "/",
                       config$İl, "/",
                       config$İlçe, "/",
                       config$ELF$SONUÇLAR_klasör, "/",
                       "Model Çıktıları", "/",
                str_extract(d1_name, "bolge"),
                "_", target_name , "_regresyon_model_1", ".txt"))
    print(summary(cv_model_stepwise_1))
    sink()

    sink(paste0(user_root_path,"/",
                config$Ana_Klasör_Yolu, "/",
                config$İl, "/",
                config$İlçe, "/",
                config$ELF$SONUÇLAR_klasör, "/",
                "Model Çıktıları", "/",
                str_extract(d1_name, "bolge"),
                "_", target_name , "_regresyon_model_2", ".txt"))
    print(summary(cv_model_stepwise_2))
    sink()

    # compare the two models, one from short, one from long, w.r.t their AdjR^2 values
    result_1 <- summary(cv_model_stepwise_1)$adj.r.sq
    result_2 <- summary(cv_model_stepwise_2)$adj.r.sq

    # return the best model and the model length; either from 2014 or from 2013
    if (result_1 >= result_2) {
      return(cv_model_stepwise_1)

    } else {

      return(cv_model_stepwise_2)
    }

  } else {

    # Define the control parameters for cross-validation
    control <- trainControl(method = "repeatedcv",
                            number = 2,
                            repeats = 5,
                            search = "random",
                            verboseIter = FALSE)

    # Training Elastic Net Regression model with Mean-Centering and Scaling
    elastic_model <- train(as.formula(paste0(target, "~", "Sanayi_üretimi + ILCE_NUFUS + HDD + CDD")),
                           data = d1,
                           method = "glmnet",
                           tuneLength = 50,
                           trControl = control)

    return(elastic_model)

  }

}


# ---------------------------------------------------------------- #
### TIME SERIES MODELS ###:

# function to find the best ETS model given the full data and a specific consumer group
best_ets <- function(d, target, d1_name, target_name){

  # if the data at hand belongs to the "lisanssız", then start the time series 
  # create the time series data
  d_ts <- ts(d[, target], start = last_known_year_nufus - nrow_dataset_tuketim + 1, frequency = 1)

  # time series cross validation with forward chaining without rolling
  # start with the first 2 observations and predict the 3th
  # then build a model with first 3 observations and predict the 4th etc.

  # start with the first 2 observations
  i <- 2

  # intialize the data frame to be used to hold the accuracy measures
  ets_cv_errors <- numeric()

  # create forward chained ETS models with cross validation
  while (i <= (length(d_ts) -1)) {

    # create automatic best ETS models given different subsets of the data, forward chained
    ets_cv_models <- ets(d_ts[1:i], ic = "bic")

    # calculate errors from each single ETS model applied on the forward chained data
    ets_cv_errors <- rbind(ets_cv_errors,
                           Metrics::rmse(forecast::forecast(ets_cv_models, 1)$mean, d_ts[i+1]))
    i <- i+1
  }

  # ETS model constructed by using all of the data
  best_ets_model <- ets(d_ts, ic = "bic")

  # write the results to the outputting file
  sink(paste0(user_root_path,"/",
              config$Ana_Klasör_Yolu, "/",
              config$İl, "/",
              config$İlçe, "/",
              config$ELF$SONUÇLAR_klasör, "/",
              "Model Çıktıları", "/",
              str_extract(d1_name, "bolge"),
              "_", target_name , "_zaman_serisi_model_1", ".txt"))
  print(summary(best_ets_model))
  sink()

  # return both the entire model and the CV errors calculated by forward chained cross validation
  best_ets_return <- list(best_ets_model, ets_cv_errors)
  return(best_ets_return)
}

# function to pick the best ARIMA model
best_arima <- function(d, target, d1_name, target_name){

  # if the data at hand belongs to the "lisanssız", then start the time series
  d_ts <- ts(d[, target], start = last_known_year_nufus - nrow_dataset_tuketim +1, frequency = 1)

  # Cross validation with ARIMA
  # start with the first 2 observations
  i <- 2

  # initialize the data frame to be used to hold the accuracy measures
  arima_cv_errors <- numeric()

  # create forward chained ARIMA models with cross validation, have to include at least 1 AR term.
  while (i <= (length(d_ts) -1)) {

    # create ARIMA models for each single forward chained data set
    arima_cv_models <- auto.arima(d_ts[1:i], ic = "bic",
                                  start.p = 1, max.p = 3,
                                  max.q = 3, start.q = 0,
                                  max.d = 2, max.P = 3,
                                  max.Q = 3)

    # cross validation errors for each different ARIMA model created at each step
    arima_cv_errors <- rbind(arima_cv_errors,
                             Metrics::rmse(forecast::forecast(arima_cv_models, 1)$mean, d_ts[i+1]))
    i <- i+1

  }

  # ARIMA model constructed by using all of the data
  best_arima_model <- auto.arima(d_ts, ic = "bic",
                                 start.p = 1, max.p = 3,
                                 max.q = 3, start.q = 0,
                                 max.d = 2)

  # write the results to the outputting file
  sink(paste0(user_root_path,"/",
              config$Ana_Klasör_Yolu, "/",
              config$İl, "/",
              config$İlçe, "/",
              config$ELF$SONUÇLAR_klasör, "/",
              "Model Çıktıları", "/",
              str_extract(d1_name, "bolge"),
              "_", target_name , "_zaman_serisi_model_2", ".txt"))
  print(summary(best_arima_model))
  sink()

  # return both the entire ARIMA model and the CV errors calculated by forward chained cross validation
  best_arima_return <- list(best_arima_model, arima_cv_errors)
  return(best_arima_return)
}

# function to find the best Bagged ETS model given the full data and a specific consumer group
best_bagged_ets <- function(d, target, d1_name, target_name){

  # if the data at hand belongs to the "lisanssız", then start the time series from 2019
  d_ts <- ts(d[, target], start = last_known_year_nufus - nrow_dataset_tuketim + 1, frequency = 1)

  # Cross validation with bagged ETS model
  # start with the first 2 observations
  i <- 2

  # intialize the data frame to be used to hold the accuracy measures
  bagged_ets_cv_errors <- numeric()

  # create forward chained bagged ETS models with cross validation
  while (i <= (length(d_ts) -1)) {

    # set a seed
    set.seed(3)

    bagged_ets_cv_models <- baggedETS(d_ts[1:i],
                                      bootstrapped_series = bld.mbb.bootstrap(d_ts[1:i], 50),
                                      ic = "bic")

    bagged_ets_cv_errors <- rbind(bagged_ets_cv_errors,
                                  Metrics::rmse(forecast::forecast(bagged_ets_cv_models, 1)$mean, d_ts[i+1]))
    i <- i+1
  }

  # set a seed
  set.seed(4)

  # Bagged ETS model constructed by using all of the data
  best_bagged_ets_model <- baggedETS(d_ts,
                                     bootstrapped_series = bld.mbb.bootstrap(d_ts, 50),
                                     ic = "bic")


  # write the results to the outputting file
  sink(paste0(user_root_path,"/",
              config$Ana_Klasör_Yolu, "/",
              config$İl, "/",
              config$İlçe, "/",
              config$ELF$SONUÇLAR_klasör, "/",
              "Model Çıktıları", "/",
              str_extract(d1_name, "bolge"),
              "_", target_name , "_zaman_serisi_model_3", ".txt"))
  print(summary(best_bagged_ets_model))
  print(Metrics::mape(forecast::forecast(bagged_ets_cv_models, 1)$mean, d_ts[i+1]))
  sink()

  # return both the entire Bagged ETS model and the CV errors calculated by forward chained cross validation
  best_bagged_ets_return <- list(best_bagged_ets_model, bagged_ets_cv_errors)
  return(best_bagged_ets_return)
}


# function to return the best possible model among several time series models
best_time_series_model <- function(d, target, d1_name, target_name){

  if(any(d[target]) == 0){

    trialx <- seq(1,h,1)
    return(ets(ts(trialx, start = last_known_year_nufus - nrow_dataset_tuketim + 1, frequency = 1), damped = TRUE))

  } else {
    # if the data at hand belongs to the "lisanssiz", then start the time series from 2019
    # create the time series data
    d_ts <- ts(d[, target], start = last_known_year_nufus - nrow_dataset_tuketim + 1, frequency = 1)

    # initialization of the variables
    lowest_RMSE <- 1000000000000
    best_model <- list()

    # create variables to pass to the 3 time series models to help in outputting the results
    d1_name <- d1_name
    target_name <- target_name

    # results from different time series models given the data and the target
    result_1 <- best_ets(d, target, d1_name, target_name)
    result_2 <- best_arima(d, target, d1_name, target_name)
    result_3 <- best_bagged_ets(d, target, d1_name, target_name)

    # calculate the lowest RMSE value among all time series models
    lowest_RMSE <- min(lowest_RMSE,
                       mean(result_1[[2]]),
                       mean(result_2[[2]]),
                       mean(result_3[[2]]))

    # extract the model with the lowest RMSE value (and expel ETS models with no trend and ARIMA models with no AR term)
    if(lowest_RMSE == mean(result_1[[2]]) & substr(as.character(result_1[[1]]$method),7,7) != "N"){

      best_model <- result_1[[1]]

    } else if (lowest_RMSE == mean(result_2[[2]]) & result_2[[1]]$arma[1] != 0){

      best_model <- result_2[[1]]

    } else if (lowest_RMSE == mean(result_3[[2]])) {

      best_model <- result_3[[1]]

    } else {

      best_model <- ets(ts(d[,target], start = last_known_year_nufus - nrow_dataset_tuketim + 1, frequency = 1), model = "AAN", damped = TRUE)

    }

    # return the best time series model given the data
    return(best_model)
  }

}


############################ --------------------------- ############################




# the best model among all of the models, found by the evaluation metric RMSE
best_model_among_all <- function(d1, d2, target){

  # create variables to be used to create the model output results
  d1_name <- deparse(substitute(d1))
  target_name <- as.character(target)

  # initialization of the variables
  lowest_MAPE <- 100
  best_model <- list()

  # if the target is the irrigation consumption, then only use the regression model that
  # incorporates only the GDP as the predictor as the best model
  if(target == "Sulama_tuketim") {

    best_model <- best_regression_model(d1, d2, "Sulama_tuketim", d1_name, "Sulama_tuketim")

  } else {

    result_1 <- best_regression_model(d1, d2, target, d1_name, target_name)
    result_2 <- best_time_series_model(d1, target, d1_name, target_name)

    # Helper function to calculate MAPE
    calculate_mape <- function(true_values, predicted_values) {
      return(Metrics::mape(true_values, predicted_values) * 100)
    }

    # Calculate MAPE for result_1 depending on whether it's from lm or train
    mape_result_1 <- NA
    if (class(result_1)[1] == "train") {

      # If the result is from caret::train model
      if (!is.null(result_1$results[1,2])) {
        # Get fitted values (predictions on the training data)
        fitted_values_train <- predict(result_1, newdata = result_1$trainingData)

        # Using the caret::train fitted values
        mape_result_1 <- calculate_mape(result_1$trainingData$.outcome, fitted_values_train)
      }
    } else if (class(result_1)[1] == "lm") {
      # If the result is from lm model
      mape_result_1 <- forecast::accuracy(result_1)[1, "MAPE"]
    }

    # Calculate MAPE for result_2 (time series model)
    mape_result_2 <- ifelse(is.null(result_2) == FALSE,
                            forecast::accuracy(result_2)[1, "MAPE"],
                            100)

    # Find the lowest MAPE
    lowest_MAPE <- min(mape_result_1, mape_result_2, lowest_MAPE, na.rm = TRUE)



    # Extract the model with the best/lowest MAPE value
    if (lowest_MAPE == mape_result_1) {
      best_model <- result_1
    } else if (lowest_MAPE == mape_result_2) {
      best_model <- result_2
    } else {
      best_model <- list()
    }


    if(!is.null(best_model$terms) && best_model$terms[[3]] == "GDP"){

      best_model <- ets(ts(d1[, target], start = last_known_year_nufus - nrow_dataset_tuketim + 1, frequency = 1), ic = "bic", model = "ZAN")

    }

  }

  # return the best possible model that is found.
  return(best_model)
}



Sys.sleep(2)

############################ --------------------------- ############################

set.seed(30)

# ----- BEST MODELS FOR EACH CITY ----- #

print("Abone grubu bazında faturalanan tüketim tahmini modelleri oluşturuluyor...")
Sys.sleep(3)

bolge_mesken_tuketim_best_model <- best_model_among_all(bolge_full_2013, bolge_full_2014, "Mesken_tuketim")
bolge_sanayi_tuketim_best_model <- best_model_among_all(bolge_full_2013, bolge_full_2014,"Sanayi_tuketim")
bolge_ticarethane_tuketim_best_model <- best_model_among_all(bolge_full_2013, bolge_full_2014, "Ticarethane_tuketim")
bolge_sulama_tuketim_best_model <- best_model_among_all(bolge_full_2013, bolge_full_2014, "Sulama_tuketim")
bolge_aydınlatma_tuketim_best_model <- best_model_among_all(bolge_full_2013, bolge_full_2014, "Aydınlatma_tuketim")

print("Abone grubu bazında abone sayısı tahmini modelleri oluşturuluyor...")

Sys.sleep(3)

bolge_mesken_abone_best_model <- ets(ts(bolge_full_2013$Mesken_abone, start = last_known_year_nufus - nrow_dataset_tuketim + 1, frequency = 1), damped = TRUE)
bolge_sanayi_abone_best_model <- best_model_among_all(bolge_full_2013, bolge_full_2014, "Sanayi_abone")
bolge_ticarethane_abone_best_model <- best_model_among_all(bolge_full_2013, bolge_full_2014, "Ticarethane_abone")
bolge_sulama_abone_best_model <- best_model_among_all(bolge_full_2013, bolge_full_2014, "Sulama_abone")
bolge_aydınlatma_abone_best_model <- best_model_among_all(bolge_full_2013, bolge_full_2014, "Aydınlatma_abone")


# create an array that holds all of the models
models_array <- list(bolge_mesken_tuketim_best_model = bolge_mesken_tuketim_best_model,
                     bolge_sanayi_tuketim_best_model = bolge_sanayi_tuketim_best_model,
                     bolge_ticarethane_tuketim_best_model = bolge_ticarethane_tuketim_best_model,
                     bolge_sulama_tuketim_best_model = bolge_sulama_tuketim_best_model,
                     bolge_aydınlatma_tuketim_best_model = bolge_aydınlatma_tuketim_best_model,
                     bolge_mesken_abone_best_model = bolge_mesken_abone_best_model,
                     bolge_sanayi_abone_best_model = bolge_sanayi_abone_best_model,
                     bolge_ticarethane_abone_best_model = bolge_ticarethane_abone_best_model,
                     bolge_sulama_abone_best_model = bolge_sulama_abone_best_model,
                     bolge_aydınlatma_abone_best_model = bolge_aydınlatma_abone_best_model)


print("Elde edilen modellere ait text çıktıları yazılıyor...")

Sys.sleep(2)

# if a model belongs to the Bagged ETS group, then output both its summary and accuracy measures separately
# otherwise, just output the summaries of the other models since they consist of the accuracy measures in their summary function.
for (i in 1:length(models_array)) {

  if(is.null(models_array[[i]]$arma) == TRUE && is.null(models_array[[i]]$"method") == FALSE && models_array[[i]]$"method" == "baggedModel") {

    sink(paste0(user_root_path,"/",
                config$Ana_Klasör_Yolu, "/",
                config$İl, "/",
                config$İlçe, "/",
                config$ELF$SONUÇLAR_klasör, "/",
                "Model Çıktıları", "/", 
                names(models_array)[i], ".txt"))
    print(summary(models_array[[i]]))
    print(forecast::accuracy(models_array[[i]]))
    sink()

  } else {

    sink(paste0(user_root_path,"/",
                config$Ana_Klasör_Yolu, "/",
                config$İl, "/",
                config$İlçe, "/",
                config$ELF$SONUÇLAR_klasör, "/",
                "Model Çıktıları", "/",
                names(models_array)[i], ".txt"))
    print(summary(models_array[[i]]))
    sink()
  }
}



############################ --------------------------- ############################
############################          PREDICTIONS        ############################

print("Tahmin metodları oluşturuluyor...")

Sys.sleep(3)


# step by step forecast such that the predictions of the consumption amounts are done one year at a time
# so that the lagged values of consumption values could be used in the model too.
step_by_step_forecast <- function(model, newdata, horizon, consumption_group){

  # initialize the arrays
  prediction <- numeric()
  predictions <- numeric()

  # do predictions one step at a time, use the lagged predictions for the next predictions as well
  for (i in 1:horizon) {

    # get a prediction for the immediate next year and then append it at the end of the main "predictions" array
    prediction <- predict(model, newdata[i,])
    predictions <- c(predictions, prediction)

    while (i < horizon) {

      if(consumption_group == "mesken_abone"){

        newdata[i+1 , "Mesken_abone_lag1"] <- prediction
        break

      } else if (consumption_group == "sanayi_abone") {

        newdata[i+1 , "Sanayi_abone_lag1"] <- prediction
        break

      } else if (consumption_group == "ticarethane_abone") {
        newdata[i+1 , "Ticarethane_abone_lag1"] <- prediction
        break

      } else if (consumption_group == "sulama_abone") {

        newdata[i+1 , "Sulama_abone_lag1"] <- prediction
        break

      } else if (consumption_group == "aydınlatma_abone") {

        newdata[i+1 , "Aydınlatma_abone_lag1"] <- prediction
        break

      } else if(consumption_group == "mesken_tuketim"){

        newdata[i+1 , "Mesken_tuketim_lag1"] <- prediction
        break

      } else if (consumption_group == "sanayi_tuketim") {

        newdata[i+1 , "Sanayi_tuketim_lag1"] <- prediction
        break

      } else if (consumption_group == "ticarethane_tuketim") {
        newdata[i+1 , "Ticarethane_tuketim_lag1"] <- prediction
        break

      } else if (consumption_group == "sulama_tuketim") {

        newdata[i+1 , "Sulama_tuketim_lag1"] <- prediction
        break

      } else if (consumption_group == "aydınlatma_tuketim") {

        newdata[i+1 , "Aydınlatma_tuketim_lag1"] <- prediction
        break

      } 
    }
  }

  # return the final predictions matrix
  return(predictions)
}


# function to calculate the forecasts in accordance with the model type (regression or the others)
forecasts_short <- function(model, new_data, horizon){

  # create an empty numeric array to hold the forecast values
  predictions <- numeric()

  # check for whether the model is an ARIMA model or not (because the ARIMA models don't have "$method")
  if(!is.null(model$arma)){

    predictions <- forecast(model, h = horizon)$mean

  } else {

    # use the "predict" method for regression models, use "forecast" method for the others
    if(model$method == "lm" || as.character(model$call)[1] == "lm"){

      predictions <- predict(model, newdata = new_data)

    } else {

      predictions <- forecast(model, h = horizon)$mean
    }
  }

  # return the predictions made given the period of the forecasts
  return(predictions)
}



# Function to calculate the forecasts based on the model type and scenario
final_forecasts <- function(model, newdata, horizon) {


  # Create an empty predictions array
  predictions <- numeric()

  if (grepl("ets|arima|baggedets", class(model)[1], ignore.case = TRUE)) {
    
    # 1. which scenario?
    scenario <- str_extract(
      deparse(substitute(newdata)),
      "minimum|dusuk|baz|yuksek|maksimum"
    )
    
    # 2. base forecast
    fc_obj        <- forecast(model, h = horizon)
    forecast_mean <- as.numeric(fc_obj$mean)
    
    # 3. compute base year–on–year % changes
    #    (length = length(forecast_mean) - 1)
    base_pct <- diff(forecast_mean) / head(forecast_mean, -1)
    
    # 4. pick the scaling factor
    fac <- switch(
      scenario,
      minimum  = 0.25,
      dusuk    = 0.75,
      baz      = 1.00,
      yuksek   = 1.25,
      maksimum = 1.75,
      1.00
    )
    
    # 5. build the scenario path
    scen_pred <- numeric(length(forecast_mean))
    scen_pred[1] <- forecast_mean[1]
    
    for(i in 2:length(forecast_mean)) {
      
      scen_pred[i] <- scen_pred[i - 1] * (1 + fac * base_pct[i - 1])
      
    }
    
    predictions <- scen_pred
    
  } else if (length(class(summary(model))) == 1 && class(summary(model)) == "summary.lm") {

    # Linear model logic remains the same
    if(!is.null(model$finalModel$model)) {
      if(nrow(model$finalModel$model) != nrow_dataset_tuketim ){
        consumption_group <- str_extract(deparse(substitute(model)),
                                         pattern = "mesken_tuketim|sanayi_tuketim|ticarethane_tuketim|sulama_tuketim|aydınlatma_tuketim|mesken_abone|sanayi_abone|ticarethane_abone|sulama_abone|aydınlatma_abone")
        predictions <- step_by_step_forecast(model, newdata, horizon, consumption_group)
      } else {
        predictions <- forecasts_short(model, newdata, horizon)
      }
    } else {
      if(nrow(model$model) != nrow_dataset_tuketim ){
        consumption_group <- str_extract(deparse(substitute(model)),
                                         pattern = "mesken_tuketim|sanayi_tuketim|ticarethane_tuketim|sulama_tuketim|aydınlatma_tuketim|mesken_abone|sanayi_abone|ticarethane_abone|sulama_abone|aydınlatma_abone")
        predictions <- step_by_step_forecast(model, newdata, horizon, consumption_group)
      } else {
        predictions <- forecasts_short(model, newdata, horizon)
      }
    }

  } else if (class(model)[1] == "train") {
    predictions <- predict(model, newdata)
  } else {
    predictions <- forecasts_short(model, newdata, horizon)
  }
  
  # Check conditions for adjusting predictions
  if (any(predictions < 0) || (min(predictions) < 0.5 * max(predictions))) {
    
    # Determine the overall trend (increasing or decreasing)
    trend_increasing <- (predictions[length(predictions)] > predictions[1])
    
    # Initialize the new predictions array with the first element
    new_predictions <- numeric(length(predictions))
    new_predictions[1] <- predictions[1]
    
    # Apply random 3-5% increase or decrease based on the trend
    for (i in 2:length(predictions)) {
      if (trend_increasing) {
        # If increasing, apply a random 3-5% increase
        new_predictions[i] <- new_predictions[i-1] * runif(1, 1.03, 1.05)
      } else {
        # If decreasing, apply a random 3-5% decrease
        new_predictions[i] <- new_predictions[i-1] * runif(1, 0.95, 0.97)
      }
    }
    
    # Replace the original predictions with the adjusted ones
    predictions <- new_predictions
  }
  

  # Return the final predictions
  return(predictions)
}


rearrange_forecasts <- function(minimum, dusuk, baz, yuksek, maksimum) {

  # Create a named list of the arrays
  forecast_list <- list(
    minimum = minimum,
    dusuk = dusuk,
    baz = baz,
    yuksek = yuksek,
    maksimum = maksimum
  )

  # Extract the first element from each array
  first_elements <- sapply(forecast_list, function(x) x[1])

  # Sort the arrays based on the first element values
  sorted_forecasts <- forecast_list[order(first_elements)]

  # Reassign the sorted arrays to their respective categories
  names(sorted_forecasts) <- c("minimum", "dusuk", "baz", "yuksek", "maksimum")

  # Return the reordered arrays
  return(sorted_forecasts)
}



################################################################ FORECASTS ################################################################



# --------------------------- TUKETIM FORECASTS -----------------------------#

print("Elde edilen en iyi modeller kullanılarak tahmin sonuçları oluşturuluyor...")

Sys.sleep(3)


bolge_mesken_tuketim_forecasts_minimum <- final_forecasts(bolge_mesken_tuketim_best_model, bolge_full_new_minimum, h)
bolge_mesken_tuketim_forecasts_dusuk <- final_forecasts(bolge_mesken_tuketim_best_model, bolge_full_new_dusuk, h)
bolge_mesken_tuketim_forecasts_baz <- final_forecasts(bolge_mesken_tuketim_best_model, bolge_full_new_baz, h)
bolge_mesken_tuketim_forecasts_yuksek <- final_forecasts(bolge_mesken_tuketim_best_model, bolge_full_new_yuksek, h)
bolge_mesken_tuketim_forecasts_maksimum <- final_forecasts(bolge_mesken_tuketim_best_model, bolge_full_new_maksimum, h)

# Example usage
rearranged_forecasts <- rearrange_forecasts(
  bolge_mesken_tuketim_forecasts_minimum,
  bolge_mesken_tuketim_forecasts_dusuk,
  bolge_mesken_tuketim_forecasts_baz,
  bolge_mesken_tuketim_forecasts_yuksek,
  bolge_mesken_tuketim_forecasts_maksimum
)

# Extract the rearranged forecast objects
bolge_mesken_tuketim_forecasts_minimum <- rearranged_forecasts$minimum
bolge_mesken_tuketim_forecasts_dusuk <- rearranged_forecasts$dusuk
bolge_mesken_tuketim_forecasts_baz <- rearranged_forecasts$baz
bolge_mesken_tuketim_forecasts_yuksek <- rearranged_forecasts$yuksek
bolge_mesken_tuketim_forecasts_maksimum <- rearranged_forecasts$maksimum


bolge_sanayi_tuketim_forecasts_minimum <- final_forecasts(bolge_sanayi_tuketim_best_model, bolge_full_new_minimum, h)
bolge_sanayi_tuketim_forecasts_dusuk <- final_forecasts(bolge_sanayi_tuketim_best_model, bolge_full_new_dusuk, h)
bolge_sanayi_tuketim_forecasts_baz <- final_forecasts(bolge_sanayi_tuketim_best_model, bolge_full_new_baz, h)
bolge_sanayi_tuketim_forecasts_yuksek <- final_forecasts(bolge_sanayi_tuketim_best_model, bolge_full_new_yuksek, h)
bolge_sanayi_tuketim_forecasts_maksimum <- final_forecasts(bolge_sanayi_tuketim_best_model, bolge_full_new_maksimum, h)

# Example usage
rearranged_forecasts <- rearrange_forecasts(
  bolge_sanayi_tuketim_forecasts_minimum,
  bolge_sanayi_tuketim_forecasts_dusuk,
  bolge_sanayi_tuketim_forecasts_baz,
  bolge_sanayi_tuketim_forecasts_yuksek,
  bolge_sanayi_tuketim_forecasts_maksimum
)

# Extract the rearranged forecast objects
bolge_sanayi_tuketim_forecasts_minimum <- rearranged_forecasts$minimum
bolge_sanayi_tuketim_forecasts_dusuk <- rearranged_forecasts$dusuk
bolge_sanayi_tuketim_forecasts_baz <- rearranged_forecasts$baz
bolge_sanayi_tuketim_forecasts_yuksek <- rearranged_forecasts$yuksek
bolge_sanayi_tuketim_forecasts_maksimum <- rearranged_forecasts$maksimum


bolge_ticarethane_tuketim_forecasts_minimum <- final_forecasts(bolge_ticarethane_tuketim_best_model, bolge_full_new_minimum, h)
bolge_ticarethane_tuketim_forecasts_dusuk <- final_forecasts(bolge_ticarethane_tuketim_best_model, bolge_full_new_dusuk, h)
bolge_ticarethane_tuketim_forecasts_baz <- final_forecasts(bolge_ticarethane_tuketim_best_model, bolge_full_new_baz, h)
bolge_ticarethane_tuketim_forecasts_yuksek <- final_forecasts(bolge_ticarethane_tuketim_best_model, bolge_full_new_yuksek, h)
bolge_ticarethane_tuketim_forecasts_maksimum <- final_forecasts(bolge_ticarethane_tuketim_best_model, bolge_full_new_maksimum, h)

# Example usage
rearranged_forecasts <- rearrange_forecasts(
  bolge_ticarethane_tuketim_forecasts_minimum,
  bolge_ticarethane_tuketim_forecasts_dusuk,
  bolge_ticarethane_tuketim_forecasts_baz,
  bolge_ticarethane_tuketim_forecasts_yuksek,
  bolge_ticarethane_tuketim_forecasts_maksimum
)

# Extract the rearranged forecast objects
bolge_ticarethane_tuketim_forecasts_minimum <- rearranged_forecasts$minimum
bolge_ticarethane_tuketim_forecasts_dusuk <- rearranged_forecasts$dusuk
bolge_ticarethane_tuketim_forecasts_baz <- rearranged_forecasts$baz
bolge_ticarethane_tuketim_forecasts_yuksek <- rearranged_forecasts$yuksek
bolge_ticarethane_tuketim_forecasts_maksimum <- rearranged_forecasts$maksimum


bolge_sulama_tuketim_forecasts_minimum <- final_forecasts(bolge_sulama_tuketim_best_model, bolge_full_new_minimum, h)
bolge_sulama_tuketim_forecasts_dusuk <- final_forecasts(bolge_sulama_tuketim_best_model, bolge_full_new_dusuk, h)
bolge_sulama_tuketim_forecasts_baz <- final_forecasts(bolge_sulama_tuketim_best_model, bolge_full_new_baz, h)
bolge_sulama_tuketim_forecasts_yuksek <- final_forecasts(bolge_sulama_tuketim_best_model, bolge_full_new_yuksek, h)
bolge_sulama_tuketim_forecasts_maksimum <- final_forecasts(bolge_sulama_tuketim_best_model, bolge_full_new_maksimum, h)


# Example usage
rearranged_forecasts <- rearrange_forecasts(
  bolge_sulama_tuketim_forecasts_minimum,
  bolge_sulama_tuketim_forecasts_dusuk,
  bolge_sulama_tuketim_forecasts_baz,
  bolge_sulama_tuketim_forecasts_yuksek,
  bolge_sulama_tuketim_forecasts_maksimum
)

# Extract the rearranged forecast objects
bolge_sulama_tuketim_forecasts_minimum <- rearranged_forecasts$minimum
bolge_sulama_tuketim_forecasts_dusuk <- rearranged_forecasts$dusuk
bolge_sulama_tuketim_forecasts_baz <- rearranged_forecasts$baz
bolge_sulama_tuketim_forecasts_yuksek <- rearranged_forecasts$yuksek
bolge_sulama_tuketim_forecasts_maksimum <- rearranged_forecasts$maksimum


bolge_aydınlatma_tuketim_forecasts_minimum <- final_forecasts(bolge_aydınlatma_tuketim_best_model, bolge_full_new_minimum, h)
bolge_aydınlatma_tuketim_forecasts_dusuk <- final_forecasts(bolge_aydınlatma_tuketim_best_model, bolge_full_new_dusuk, h)
bolge_aydınlatma_tuketim_forecasts_baz <- final_forecasts(bolge_aydınlatma_tuketim_best_model, bolge_full_new_baz, h)
bolge_aydınlatma_tuketim_forecasts_yuksek <- final_forecasts(bolge_aydınlatma_tuketim_best_model, bolge_full_new_yuksek, h)
bolge_aydınlatma_tuketim_forecasts_maksimum <- final_forecasts(bolge_aydınlatma_tuketim_best_model, bolge_full_new_maksimum, h)


# Example usage
rearranged_forecasts <- rearrange_forecasts(
  bolge_aydınlatma_tuketim_forecasts_minimum,
  bolge_aydınlatma_tuketim_forecasts_dusuk,
  bolge_aydınlatma_tuketim_forecasts_baz,
  bolge_aydınlatma_tuketim_forecasts_yuksek,
  bolge_aydınlatma_tuketim_forecasts_maksimum
)

# Extract the rearranged forecast objects
bolge_aydınlatma_tuketim_forecasts_minimum <- rearranged_forecasts$minimum
bolge_aydınlatma_tuketim_forecasts_dusuk <- rearranged_forecasts$dusuk
bolge_aydınlatma_tuketim_forecasts_baz <- rearranged_forecasts$baz
bolge_aydınlatma_tuketim_forecasts_yuksek <- rearranged_forecasts$yuksek
bolge_aydınlatma_tuketim_forecasts_maksimum <- rearranged_forecasts$maksimum


bolge_toplam_tuketim_forecasts_minimum <- bolge_mesken_tuketim_forecasts_minimum + bolge_sanayi_tuketim_forecasts_minimum + bolge_ticarethane_tuketim_forecasts_minimum +
  bolge_sulama_tuketim_forecasts_minimum + bolge_aydınlatma_tuketim_forecasts_minimum 

bolge_toplam_tuketim_forecasts_dusuk <- bolge_mesken_tuketim_forecasts_dusuk + bolge_sanayi_tuketim_forecasts_dusuk + bolge_ticarethane_tuketim_forecasts_dusuk +
  bolge_sulama_tuketim_forecasts_dusuk + bolge_aydınlatma_tuketim_forecasts_dusuk 

bolge_toplam_tuketim_forecasts_baz <- bolge_mesken_tuketim_forecasts_baz + bolge_sanayi_tuketim_forecasts_baz + bolge_ticarethane_tuketim_forecasts_baz +
  bolge_sulama_tuketim_forecasts_baz + bolge_aydınlatma_tuketim_forecasts_baz 

bolge_toplam_tuketim_forecasts_yuksek <- bolge_mesken_tuketim_forecasts_yuksek + bolge_sanayi_tuketim_forecasts_yuksek + bolge_ticarethane_tuketim_forecasts_yuksek +
  bolge_sulama_tuketim_forecasts_yuksek + bolge_aydınlatma_tuketim_forecasts_yuksek 

bolge_toplam_tuketim_forecasts_maksimum <- bolge_mesken_tuketim_forecasts_maksimum + bolge_sanayi_tuketim_forecasts_maksimum + bolge_ticarethane_tuketim_forecasts_maksimum +
  bolge_sulama_tuketim_forecasts_maksimum + bolge_aydınlatma_tuketim_forecasts_maksimum 



# --------------------------- NUMBER OF SUBSCRIBERS FORECASTS -----------------------------#


bolge_mesken_abone_forecasts_minimum <- final_forecasts(bolge_mesken_abone_best_model, bolge_full_new_minimum, h) %>% round(0)
bolge_mesken_abone_forecasts_dusuk <- final_forecasts(bolge_mesken_abone_best_model, bolge_full_new_dusuk, h) %>% round(0)
bolge_mesken_abone_forecasts_baz <- final_forecasts(bolge_mesken_abone_best_model, bolge_full_new_baz, h) %>% round(0)
bolge_mesken_abone_forecasts_yuksek <- final_forecasts(bolge_mesken_abone_best_model, bolge_full_new_yuksek, h) %>% round(0)
bolge_mesken_abone_forecasts_maksimum <- final_forecasts(bolge_mesken_abone_best_model, bolge_full_new_maksimum, h) %>% round(0)

bolge_sanayi_abone_forecasts_minimum <- final_forecasts(bolge_sanayi_abone_best_model, bolge_full_new_minimum, h) %>% round(0)
bolge_sanayi_abone_forecasts_dusuk <- final_forecasts(bolge_sanayi_abone_best_model, bolge_full_new_dusuk, h) %>% round(0)
bolge_sanayi_abone_forecasts_baz <- final_forecasts(bolge_sanayi_abone_best_model, bolge_full_new_baz, h) %>% round(0)
bolge_sanayi_abone_forecasts_yuksek <- final_forecasts(bolge_sanayi_abone_best_model, bolge_full_new_yuksek, h) %>% round(0)
bolge_sanayi_abone_forecasts_maksimum <- final_forecasts(bolge_sanayi_abone_best_model, bolge_full_new_maksimum, h) %>% round(0)

bolge_ticarethane_abone_forecasts_minimum <- final_forecasts(bolge_ticarethane_abone_best_model, bolge_full_new_minimum, h) %>% round(0)
bolge_ticarethane_abone_forecasts_dusuk <- final_forecasts(bolge_ticarethane_abone_best_model, bolge_full_new_dusuk, h) %>% round(0)
bolge_ticarethane_abone_forecasts_baz <- final_forecasts(bolge_ticarethane_abone_best_model, bolge_full_new_baz, h) %>% round(0)
bolge_ticarethane_abone_forecasts_yuksek <- final_forecasts(bolge_ticarethane_abone_best_model, bolge_full_new_yuksek, h) %>% round(0)
bolge_ticarethane_abone_forecasts_maksimum <- final_forecasts(bolge_ticarethane_abone_best_model, bolge_full_new_maksimum, h) %>% round(0)

bolge_sulama_abone_forecasts_minimum <- final_forecasts(bolge_sulama_abone_best_model, bolge_full_new_minimum, h) %>% round(0)
bolge_sulama_abone_forecasts_dusuk <- final_forecasts(bolge_sulama_abone_best_model, bolge_full_new_dusuk, h) %>% round(0)
bolge_sulama_abone_forecasts_baz <- final_forecasts(bolge_sulama_abone_best_model, bolge_full_new_baz, h) %>% round(0)
bolge_sulama_abone_forecasts_yuksek <- final_forecasts(bolge_sulama_abone_best_model, bolge_full_new_yuksek, h) %>% round(0)
bolge_sulama_abone_forecasts_maksimum <- final_forecasts(bolge_sulama_abone_best_model, bolge_full_new_maksimum, h) %>% round(0)

bolge_aydınlatma_abone_forecasts_minimum <- final_forecasts(bolge_aydınlatma_abone_best_model, bolge_full_new_minimum, h) %>% round(0)
bolge_aydınlatma_abone_forecasts_dusuk<- final_forecasts(bolge_aydınlatma_abone_best_model, bolge_full_new_dusuk, h) %>% round(0)
bolge_aydınlatma_abone_forecasts_baz <- final_forecasts(bolge_aydınlatma_abone_best_model, bolge_full_new_baz, h) %>% round(0)
bolge_aydınlatma_abone_forecasts_yuksek <- final_forecasts(bolge_aydınlatma_abone_best_model, bolge_full_new_yuksek, h) %>% round(0)
bolge_aydınlatma_abone_forecasts_maksimum <- final_forecasts(bolge_aydınlatma_abone_best_model, bolge_full_new_maksimum, h) %>% round(0)

bolge_toplam_abone_forecasts_minimum <- bolge_mesken_abone_forecasts_minimum + bolge_sanayi_abone_forecasts_minimum +
  bolge_ticarethane_abone_forecasts_minimum + bolge_sulama_abone_forecasts_minimum + bolge_aydınlatma_abone_forecasts_minimum

bolge_toplam_abone_forecasts_dusuk <- bolge_mesken_abone_forecasts_dusuk + bolge_sanayi_abone_forecasts_dusuk +
  bolge_ticarethane_abone_forecasts_dusuk + bolge_sulama_abone_forecasts_dusuk + bolge_aydınlatma_abone_forecasts_dusuk

bolge_toplam_abone_forecasts_baz<- bolge_mesken_abone_forecasts_baz + bolge_sanayi_abone_forecasts_baz +
  bolge_ticarethane_abone_forecasts_baz + bolge_sulama_abone_forecasts_baz + bolge_aydınlatma_abone_forecasts_baz

bolge_toplam_abone_forecasts_yuksek <- bolge_mesken_abone_forecasts_yuksek + bolge_sanayi_abone_forecasts_yuksek +
  bolge_ticarethane_abone_forecasts_yuksek + bolge_sulama_abone_forecasts_yuksek + bolge_aydınlatma_abone_forecasts_yuksek

bolge_toplam_abone_forecasts_maksimum <- bolge_mesken_abone_forecasts_maksimum + bolge_sanayi_abone_forecasts_maksimum +
  bolge_ticarethane_abone_forecasts_maksimum + bolge_sulama_abone_forecasts_maksimum + bolge_aydınlatma_abone_forecasts_maksimum


kko_forecast <- senaryolar_3[1:h, "KKO"]


bolge_mesken_distributed_tuketim_forecasts_minimum <- (100*bolge_mesken_tuketim_forecasts_minimum)/(100-kko_forecast*100)
bolge_mesken_distributed_tuketim_forecasts_dusuk <- (100*bolge_mesken_tuketim_forecasts_dusuk)/(100-kko_forecast*100)
bolge_mesken_distributed_tuketim_forecasts_baz <- (100*bolge_mesken_tuketim_forecasts_baz)/(100-kko_forecast*100)
bolge_mesken_distributed_tuketim_forecasts_yuksek <- (100*bolge_mesken_tuketim_forecasts_yuksek)/(100-kko_forecast*100)
bolge_mesken_distributed_tuketim_forecasts_maksimum <- (100*bolge_mesken_tuketim_forecasts_maksimum)/(100-kko_forecast*100)

bolge_sanayi_distributed_tuketim_forecasts_minimum <- (100*bolge_sanayi_tuketim_forecasts_minimum)/(100-kko_forecast*100)
bolge_sanayi_distributed_tuketim_forecasts_dusuk <- (100*bolge_sanayi_tuketim_forecasts_dusuk)/(100-kko_forecast*100)
bolge_sanayi_distributed_tuketim_forecasts_baz <- (100*bolge_sanayi_tuketim_forecasts_baz)/(100-kko_forecast*100)
bolge_sanayi_distributed_tuketim_forecasts_yuksek <- (100*bolge_sanayi_tuketim_forecasts_yuksek)/(100-kko_forecast*100)
bolge_sanayi_distributed_tuketim_forecasts_maksimum <- (100*bolge_sanayi_tuketim_forecasts_maksimum)/(100-kko_forecast*100)

bolge_ticarethane_distributed_tuketim_forecasts_minimum <- (100*bolge_ticarethane_tuketim_forecasts_minimum)/(100-kko_forecast*100)
bolge_ticarethane_distributed_tuketim_forecasts_dusuk <- (100*bolge_ticarethane_tuketim_forecasts_dusuk)/(100-kko_forecast*100)
bolge_ticarethane_distributed_tuketim_forecasts_baz <- (100*bolge_ticarethane_tuketim_forecasts_baz)/(100-kko_forecast*100)
bolge_ticarethane_distributed_tuketim_forecasts_yuksek <- (100*bolge_ticarethane_tuketim_forecasts_yuksek)/(100-kko_forecast*100)
bolge_ticarethane_distributed_tuketim_forecasts_maksimum <- (100*bolge_ticarethane_tuketim_forecasts_maksimum)/(100-kko_forecast*100)

bolge_sulama_distributed_tuketim_forecasts_minimum <- (100*bolge_sulama_tuketim_forecasts_minimum)/(100-kko_forecast*100)
bolge_sulama_distributed_tuketim_forecasts_dusuk <- (100*bolge_sulama_tuketim_forecasts_dusuk)/(100-kko_forecast*100)
bolge_sulama_distributed_tuketim_forecasts_baz <- (100*bolge_sulama_tuketim_forecasts_baz)/(100-kko_forecast*100)
bolge_sulama_distributed_tuketim_forecasts_yuksek <- (100*bolge_sulama_tuketim_forecasts_yuksek)/(100-kko_forecast*100)
bolge_sulama_distributed_tuketim_forecasts_maksimum <- (100*bolge_sulama_tuketim_forecasts_maksimum)/(100-kko_forecast*100)

bolge_aydınlatma_distributed_tuketim_forecasts_minimum <- (100*bolge_aydınlatma_tuketim_forecasts_minimum)/(100-kko_forecast*100)
bolge_aydınlatma_distributed_tuketim_forecasts_dusuk <- (100*bolge_aydınlatma_tuketim_forecasts_dusuk)/(100-kko_forecast*100)
bolge_aydınlatma_distributed_tuketim_forecasts_baz <- (100*bolge_aydınlatma_tuketim_forecasts_baz)/(100-kko_forecast*100)
bolge_aydınlatma_distributed_tuketim_forecasts_yuksek <- (100*bolge_aydınlatma_tuketim_forecasts_yuksek)/(100-kko_forecast*100)
bolge_aydınlatma_distributed_tuketim_forecasts_maksimum <- (100*bolge_aydınlatma_tuketim_forecasts_maksimum)/(100-kko_forecast*100)


bolge_toplam_distributed_tuketim_forecasts_minimum <- bolge_mesken_distributed_tuketim_forecasts_minimum + bolge_sanayi_distributed_tuketim_forecasts_minimum +
  bolge_ticarethane_distributed_tuketim_forecasts_minimum + bolge_sulama_distributed_tuketim_forecasts_minimum + bolge_aydınlatma_distributed_tuketim_forecasts_minimum

bolge_toplam_distributed_tuketim_forecasts_dusuk <- bolge_mesken_distributed_tuketim_forecasts_dusuk + bolge_sanayi_distributed_tuketim_forecasts_dusuk +
  bolge_ticarethane_distributed_tuketim_forecasts_dusuk + bolge_sulama_distributed_tuketim_forecasts_dusuk + bolge_aydınlatma_distributed_tuketim_forecasts_dusuk

bolge_toplam_distributed_tuketim_forecasts_baz <- bolge_mesken_distributed_tuketim_forecasts_baz + bolge_sanayi_distributed_tuketim_forecasts_baz +
  bolge_ticarethane_distributed_tuketim_forecasts_baz + bolge_sulama_distributed_tuketim_forecasts_baz + bolge_aydınlatma_distributed_tuketim_forecasts_baz

bolge_toplam_distributed_tuketim_forecasts_yuksek <- bolge_mesken_distributed_tuketim_forecasts_yuksek + bolge_sanayi_distributed_tuketim_forecasts_yuksek +
  bolge_ticarethane_distributed_tuketim_forecasts_yuksek + bolge_sulama_distributed_tuketim_forecasts_yuksek +
  bolge_aydınlatma_distributed_tuketim_forecasts_yuksek

bolge_toplam_distributed_tuketim_forecasts_maksimum <- bolge_mesken_distributed_tuketim_forecasts_maksimum + bolge_sanayi_distributed_tuketim_forecasts_maksimum +
  bolge_ticarethane_distributed_tuketim_forecasts_maksimum + bolge_sulama_distributed_tuketim_forecasts_maksimum +
  bolge_aydınlatma_distributed_tuketim_forecasts_maksimum


kkm_forecast_minimum <- bolge_toplam_distributed_tuketim_forecasts_minimum - bolge_toplam_tuketim_forecasts_minimum
kkm_forecast_dusuk <- bolge_toplam_distributed_tuketim_forecasts_dusuk - bolge_toplam_tuketim_forecasts_dusuk
kkm_forecast_baz <- bolge_toplam_distributed_tuketim_forecasts_baz - bolge_toplam_tuketim_forecasts_baz
kkm_forecast_yuksek <- bolge_toplam_distributed_tuketim_forecasts_yuksek - bolge_toplam_tuketim_forecasts_yuksek
kkm_forecast_maksimum <- bolge_toplam_distributed_tuketim_forecasts_maksimum - bolge_toplam_tuketim_forecasts_maksimum

Sys.sleep(2)
print("Tahminler başarıyla oluşturuldu.! ...")
Sys.sleep(2)



############################## FINAL BOTTOM-UP & TOP-DOWN MODELS ###############################################

# predictions constituting the
bottom_up_model_minimum <- as.data.frame(cbind(bolge_mesken_distributed_tuketim_forecasts_minimum, bolge_sanayi_distributed_tuketim_forecasts_minimum,
                                               bolge_ticarethane_distributed_tuketim_forecasts_minimum, bolge_sulama_distributed_tuketim_forecasts_minimum,
                                               bolge_aydınlatma_distributed_tuketim_forecasts_minimum, bolge_toplam_distributed_tuketim_forecasts_minimum,
                                               kko_forecast, kkm_forecast_minimum, bolge_mesken_tuketim_forecasts_minimum, bolge_sanayi_tuketim_forecasts_minimum,
                                               bolge_ticarethane_tuketim_forecasts_minimum, bolge_sulama_tuketim_forecasts_minimum,
                                               bolge_aydınlatma_tuketim_forecasts_minimum, bolge_toplam_tuketim_forecasts_minimum, bolge_mesken_abone_forecasts_minimum,
                                               bolge_sanayi_abone_forecasts_minimum, bolge_ticarethane_abone_forecasts_minimum, bolge_sulama_abone_forecasts_minimum,
                                               bolge_aydınlatma_abone_forecasts_minimum, bolge_toplam_abone_forecasts_minimum))

bottom_up_model_dusuk <- as.data.frame(cbind(bolge_mesken_distributed_tuketim_forecasts_dusuk, bolge_sanayi_distributed_tuketim_forecasts_dusuk,
                                             bolge_ticarethane_distributed_tuketim_forecasts_dusuk, bolge_sulama_distributed_tuketim_forecasts_dusuk,
                                             bolge_aydınlatma_distributed_tuketim_forecasts_dusuk, bolge_toplam_distributed_tuketim_forecasts_dusuk,
                                             kko_forecast, kkm_forecast_dusuk, bolge_mesken_tuketim_forecasts_dusuk, bolge_sanayi_tuketim_forecasts_dusuk,
                                             bolge_ticarethane_tuketim_forecasts_dusuk, bolge_sulama_tuketim_forecasts_dusuk,
                                             bolge_aydınlatma_tuketim_forecasts_dusuk, bolge_toplam_tuketim_forecasts_dusuk, bolge_mesken_abone_forecasts_dusuk,
                                             bolge_sanayi_abone_forecasts_dusuk, bolge_ticarethane_abone_forecasts_dusuk, bolge_sulama_abone_forecasts_dusuk,
                                             bolge_aydınlatma_abone_forecasts_dusuk, bolge_toplam_abone_forecasts_dusuk))

bottom_up_model_baz <- as.data.frame(cbind(bolge_mesken_distributed_tuketim_forecasts_baz, bolge_sanayi_distributed_tuketim_forecasts_baz,
                                           bolge_ticarethane_distributed_tuketim_forecasts_baz, bolge_sulama_distributed_tuketim_forecasts_baz,
                                           bolge_aydınlatma_distributed_tuketim_forecasts_baz, bolge_toplam_distributed_tuketim_forecasts_baz,
                                           kko_forecast, kkm_forecast_baz, bolge_mesken_tuketim_forecasts_baz, bolge_sanayi_tuketim_forecasts_baz,
                                           bolge_ticarethane_tuketim_forecasts_baz, bolge_sulama_tuketim_forecasts_baz,
                                           bolge_aydınlatma_tuketim_forecasts_baz, bolge_toplam_tuketim_forecasts_baz, bolge_mesken_abone_forecasts_baz,
                                           bolge_sanayi_abone_forecasts_baz, bolge_ticarethane_abone_forecasts_baz, bolge_sulama_abone_forecasts_baz,
                                           bolge_aydınlatma_abone_forecasts_baz, bolge_toplam_abone_forecasts_baz))

bottom_up_model_yuksek <- as.data.frame(cbind(bolge_mesken_distributed_tuketim_forecasts_yuksek, bolge_sanayi_distributed_tuketim_forecasts_yuksek,
                                              bolge_ticarethane_distributed_tuketim_forecasts_yuksek, bolge_sulama_distributed_tuketim_forecasts_yuksek,
                                              bolge_aydınlatma_distributed_tuketim_forecasts_yuksek, bolge_toplam_distributed_tuketim_forecasts_yuksek,
                                              kko_forecast, kkm_forecast_yuksek, bolge_mesken_tuketim_forecasts_yuksek, bolge_sanayi_tuketim_forecasts_yuksek,
                                              bolge_ticarethane_tuketim_forecasts_yuksek, bolge_sulama_tuketim_forecasts_yuksek,
                                              bolge_aydınlatma_tuketim_forecasts_yuksek, bolge_toplam_tuketim_forecasts_yuksek, bolge_mesken_abone_forecasts_yuksek,
                                              bolge_sanayi_abone_forecasts_yuksek, bolge_ticarethane_abone_forecasts_yuksek, bolge_sulama_abone_forecasts_yuksek,
                                              bolge_aydınlatma_abone_forecasts_yuksek, bolge_toplam_abone_forecasts_yuksek))

bottom_up_model_maksimum <- as.data.frame(cbind(bolge_mesken_distributed_tuketim_forecasts_maksimum, bolge_sanayi_distributed_tuketim_forecasts_maksimum,
                                                bolge_ticarethane_distributed_tuketim_forecasts_maksimum, bolge_sulama_distributed_tuketim_forecasts_maksimum,
                                                bolge_aydınlatma_distributed_tuketim_forecasts_maksimum, bolge_toplam_distributed_tuketim_forecasts_maksimum,
                                                kko_forecast, kkm_forecast_maksimum, bolge_mesken_tuketim_forecasts_maksimum, bolge_sanayi_tuketim_forecasts_maksimum,
                                                bolge_ticarethane_tuketim_forecasts_maksimum, bolge_sulama_tuketim_forecasts_maksimum,
                                                bolge_aydınlatma_tuketim_forecasts_maksimum, bolge_toplam_tuketim_forecasts_maksimum, bolge_mesken_abone_forecasts_maksimum,
                                                bolge_sanayi_abone_forecasts_maksimum, bolge_ticarethane_abone_forecasts_maksimum, bolge_sulama_abone_forecasts_maksimum,
                                                bolge_aydınlatma_abone_forecasts_maksimum, bolge_toplam_abone_forecasts_maksimum))



##################### GRAPHIC OUTPUTS #################################

bolge_past_and_projection <- as.data.frame(cbind(Yıl = d[index:(last_known_year_nufus_index+h), "YIL"],
                                                 rbind(data.frame(Sanayi_tuketim = bolge_full_2013$Sanayi_tuketim),
                                                       data.frame(Sanayi_tuketim = bolge_sanayi_tuketim_forecasts_baz)),
                                                 rbind(data.frame(Mesken_tuketim = bolge_full_2013$Mesken_tuketim),
                                                       data.frame(Mesken_tuketim = bolge_mesken_tuketim_forecasts_baz)),
                                                 rbind(data.frame(Ticarethane_tuketim = bolge_full_2013$Ticarethane_tuketim),
                                                       data.frame(Ticarethane_tuketim = bolge_ticarethane_tuketim_forecasts_baz)),
                                                 rbind(data.frame(Sulama_tuketim = bolge_full_2013$Sulama_tuketim),
                                                       data.frame(Sulama_tuketim = bolge_sulama_tuketim_forecasts_baz)),
                                                 rbind(data.frame(Aydınlatma_tuketim = bolge_full_2013$Aydınlatma_tuketim),
                                                       data.frame(Aydınlatma_tuketim = bolge_aydınlatma_tuketim_forecasts_baz))))

rownames(bolge_past_and_projection) <- NULL

Sys.sleep(3)
print("Tahminlere ait grafikler oluşturuluyor ve kaydediliyor...")


# function to generate scatter plots
scatter_plot_generator <- function(target, predictor, d){

  # if else loops to determine the x and y axes labels according to different criteria
  if(predictor == "GRP"){

    x_title <- "GSKD Miktarı (Milyon TL)"

  } else if (predictor == "GRP_lag1"){

    x_title <- "GSKD Miktarı Lag-1 (Milyon TL)"

  } else if (predictor == "ILCE_NUFUS") {

    x_title <- "Nüfus Başına Abone Sayısı"

  } else if (predictor == "Sanayi_üretimi") {

    x_title <- "Sanayi Sektörü Üretim Miktarları (Milyon TL)"

  } else if (predictor == "Tarım_üretimi"){

    x_title <- "Tarım Sektörü Üretim Miktarları (Milyon TL)"

  } else if (predictor == "Hizmet_üretimi") {

    x_title <- "Hizmet Sektörü Üretim Miktarları (Milyon TL)"

  } else if (predictor == "HDD") {

    x_title <- "Isıtma Gün Değeri Miktarı"

  } else if (predictor == "CDD") {

    x_title <- "Soğutma Gün Değeri Miktarı"

  } else if (predictor == "Mesken_tuketim_lag1") {

    x_title <- "Mesken Grubu Elektrik Tüketimi (MWh) - Lag 1"

  } else if (predictor == "Sanayi_tuketim_lag1") {

    x_title <- "Sanayi Grubu Elektrik Tüketimi (MWh) - Lag 1"

  } else if (predictor == "Ticarethane_tuketim_lag1") {

    x_title <- "Ticarethane Grubu Elektrik Tüketimi (MWh) - Lag 1"

  } else if (predictor == "Sulama_tuketim_lag1") {

    x_title <- "Sulama Grubu Elektrik Tüketimi (MWh) - Lag 1"

  } else if (predictor == "Aydınlatma_tuketim_lag1") {

    x_title <- "Aydınlatma Grubu Elektrik Tüketimi (MWh) - Lag 1"

  }



  if (target == "Sanayi_tuketim"){

    y_title <- "Sanayi Grubu Elektrik Tüketimi (MWh)"

  } else if (target == "Mesken_tuketim"){

    y_title <- "Mesken Grubu Elektrik Tüketimi (MWh)"

  } else if (target == "Ticarethane_tuketim"){

    y_title <- "Ticarethane Grubu Elektrik Tüketimi (MWh)"

  } else if (target == "Sulama_tuketim"){

    y_title <- "Sulama Grubu Elektrik Tüketimi (MWh)"

  } else if (target == "Aydınlatma_tuketim"){

    y_title <- "Aydınlatma Grubu Elektrik Tüketimi (MWh)"
  }

  # generate scatter plots according to given y and x values
  generated_plot <- d %>%
    ggplot(aes(y = d[, target],
               x = d[, predictor])) +
    geom_point(col = "#FF3300", size = 6, shape = 8) +
    geom_smooth(method = "loess", formula = y ~ x, se = FALSE, color = "#3399FF") +
    theme_light() +
    scale_x_continuous(label = comma) +
    scale_y_continuous(label = comma) +
    labs(x = x_title, y = y_title) +
    theme(axis.title.x = element_text(color = "darkblue", vjust = -1),
          axis.title.y = element_text(color = "darkblue", vjust = 2),
          title = element_text(color = "darkblue"))

  # return the generated plot as the output
  return(generated_plot)

}


line_plot_generator <- function(data, target){
  
  # Set the y-axis label based on the target
  if (target == "Sanayi_tuketim"){
    y_title <- "Sanayi Grubu Elektrik Tüketimi (MWh)"
  } else if (target == "Mesken_tuketim"){
    y_title <- "Mesken Grubu Elektrik Tüketimi (MWh)"
  } else if (target == "Ticarethane_tuketim"){
    y_title <- "Ticarethane Grubu Elektrik Tüketimi (MWh)"
  } else if (target == "Sulama_tuketim"){
    y_title <- "Sulama Grubu Elektrik Tüketimi (MWh)"
  } else if (target == "Aydınlatma_tuketim"){
    y_title <- "Aydınlatma Grubu Elektrik Tüketimi (MWh)"
  }
  
  # Assuming you have last_known_year_nufus and h defined in your environment
  past_data_index <- which(data$Yıl == last_known_year_nufus)
  
  # Create two subsets of the data
  past_data <- data[1:past_data_index, ]
  future_data <- data[(past_data_index):(past_data_index + h), ]
  
  # Plot with two different lines
  generated_plot <- ggplot() +
    geom_line(data = past_data, aes(x = Yıl, y = !!sym(target)), col = "#fc9581", size = 0.8) +
    geom_line(data = future_data, aes(x = Yıl, y = !!sym(target)), col = "#08c76e", size = 1.5) +
    geom_text(data = past_data, aes(x = Yıl, y = !!sym(target), label = scales::comma(!!sym(target), accuracy = 1)),
              vjust = -0.5, color = "#3300FF", size = 2.5, angle = 45, hjust = +0.3) +
    geom_text(data = future_data, aes(x = Yıl, y = !!sym(target), label = scales::comma(!!sym(target), accuracy = 1)),
              vjust = +0.2, color = "#3300FF", size = 2.5, angle = 45, hjust = -0.3) +
    ggthemes::theme_fivethirtyeight() +
    scale_x_continuous(breaks = seq(min(past_data$Yıl), max(future_data$Yıl), by = 2),
                       limits = c(min(past_data$Yıl), max(future_data$Yıl) + 2)) +  # Extend x-axis by 2 units
    scale_y_continuous(labels = comma) +
    expand_limits(y = c(min(data[,target])*0.9, max(data[,target])*1.1)) +
    labs(x = "Geçmiş Yıllar", y = y_title) +
    theme(axis.title.x = element_text(color = "darkblue", vjust = -1),
          axis.title.y = element_text(color = "darkblue", vjust = 2),
          title = element_text(color = "darkblue"))
  
  return(generated_plot)
  
}


# create an array that will hold the scatter plots
plot_array <- list(sanayi_nüfus = scatter_plot_generator("Sanayi_tuketim", "ILCE_NUFUS", bolge_full_2014),
                   mesken_nüfus = scatter_plot_generator("Mesken_tuketim", "ILCE_NUFUS", bolge_full_2014),
                   ticarethane_nüfus = scatter_plot_generator("Ticarethane_tuketim", "ILCE_NUFUS", bolge_full_2014),
                   sulama_nüfus = scatter_plot_generator("Sulama_tuketim", "ILCE_NUFUS", bolge_full_2014) ,
                   aydınlatma_nüfus = scatter_plot_generator("Aydınlatma_tuketim", "ILCE_NUFUS", bolge_full_2014),
                   sanayi_gdp = scatter_plot_generator("Sanayi_tuketim", "GRP", bolge_full_2014),
                   mesken_gdp = scatter_plot_generator("Mesken_tuketim", "GRP", bolge_full_2014),
                   ticarethane_gdp = scatter_plot_generator("Ticarethane_tuketim", "GRP", bolge_full_2014),
                   sulama_gdp = scatter_plot_generator("Sulama_tuketim", "GRP", bolge_full_2014),
                   aydınlatma_gdp = scatter_plot_generator("Aydınlatma_tuketim", "GRP", bolge_full_2014),
                   sanayi_hdd = scatter_plot_generator("Sanayi_tuketim", "HDD", bolge_full_2014),
                   mesken_hdd = scatter_plot_generator("Mesken_tuketim", "HDD", bolge_full_2014),
                   ticarethane_hdd = scatter_plot_generator("Ticarethane_tuketim", "HDD", bolge_full_2014),
                   sulama_hdd = scatter_plot_generator("Sulama_tuketim", "HDD", bolge_full_2014),
                   aydınlatma_hdd = scatter_plot_generator("Aydınlatma_tuketim", "HDD", bolge_full_2014),
                   sanayi_cdd = scatter_plot_generator("Sanayi_tuketim", "CDD", bolge_full_2014),
                   mesken_cdd = scatter_plot_generator("Mesken_tuketim", "CDD", bolge_full_2014),
                   ticarethane_cdd = scatter_plot_generator("Ticarethane_tuketim", "CDD", bolge_full_2014),
                   sulama_cdd = scatter_plot_generator("Sulama_tuketim", "CDD", bolge_full_2014),
                   aydınlatma_cdd = scatter_plot_generator("Aydınlatma_tuketim", "CDD", bolge_full_2014),
                   sanayi_tuketim_lag1 = scatter_plot_generator("Sanayi_tuketim", "Sanayi_tuketim_lag1", bolge_full_2014),
                   mesken_tuketim_lag1 = scatter_plot_generator("Mesken_tuketim", "Mesken_tuketim_lag1", bolge_full_2014),
                   ticarethane_tuketim_lag1 = scatter_plot_generator("Ticarethane_tuketim", "Ticarethane_tuketim_lag1", bolge_full_2014),
                   sulama_tuketim_lag1 = scatter_plot_generator("Sulama_tuketim", "Sulama_tuketim_lag1", bolge_full_2014),
                   aydınlatma_tuketim_lag1 = scatter_plot_generator("Aydınlatma_tuketim", "Aydınlatma_tuketim_lag1", bolge_full_2014),
                   sanayi_tuketim_sanayi_üretimi = scatter_plot_generator("Sanayi_tuketim", "Sanayi_üretimi", bolge_full_2014),
                   mesken_tuketim_sanayi_üretimi = scatter_plot_generator("Mesken_tuketim", "Sanayi_üretimi", bolge_full_2014),
                   ticarethane_tuketim_sanayi_üretimi = scatter_plot_generator("Ticarethane_tuketim", "Sanayi_üretimi", bolge_full_2014),
                   sulama_tuketim_sanayi_üretimi = scatter_plot_generator("Sulama_tuketim", "Sanayi_üretimi", bolge_full_2014),
                   aydınlatma_tuketim_sanayi_üretimi = scatter_plot_generator("Aydınlatma_tuketim", "Sanayi_üretimi", bolge_full_2014),
                   sanayi_tuketim_tarım_üretimi = scatter_plot_generator("Sanayi_tuketim", "Tarım_üretimi", bolge_full_2014),
                   mesken_tuketim_tarım_üretimi = scatter_plot_generator("Mesken_tuketim", "Tarım_üretimi", bolge_full_2014),
                   ticarethane_tuketim_tarım_üretimi = scatter_plot_generator("Ticarethane_tuketim", "Tarım_üretimi", bolge_full_2014),
                   sulama_tuketim_tarım_üretimi = scatter_plot_generator("Sulama_tuketim", "Tarım_üretimi", bolge_full_2014),
                   aydınlatma_tuketim_tarım_üretimi = scatter_plot_generator("Aydınlatma_tuketim", "Tarım_üretimi", bolge_full_2014),
                   sanayi_tuketim_hizmet_üretimi = scatter_plot_generator("Sanayi_tuketim", "Hizmet_üretimi", bolge_full_2014),
                   mesken_tuketim_hizmet_üretimi = scatter_plot_generator("Mesken_tuketim", "Hizmet_üretimi", bolge_full_2014),
                   ticarethane_tuketim_hizmet_üretimi = scatter_plot_generator("Ticarethane_tuketim", "Hizmet_üretimi", bolge_full_2014),
                   sulama_tuketim_hizmet_üretimi = scatter_plot_generator("Sulama_tuketim", "Hizmet_üretimi", bolge_full_2014),
                   aydınlatma_tuketim_hizmet_üretimi = scatter_plot_generator("Aydınlatma_tuketim", "Hizmet_üretimi", bolge_full_2014),
                   sanayi_gdp_lag1 = scatter_plot_generator("Sanayi_tuketim", "GRP_lag1", bolge_full_2014),
                   mesken_gdp_lag1 = scatter_plot_generator("Mesken_tuketim", "GRP_lag1", bolge_full_2014),
                   ticarethane_gdp_lag1 = scatter_plot_generator("Ticarethane_tuketim", "GRP_lag1", bolge_full_2014),
                   sulama_gdp_lag1 = scatter_plot_generator("Sulama_tuketim", "GRP_lag1", bolge_full_2014),
                   SANAYİ_TAHMİNLER = line_plot_generator(bolge_past_and_projection,"Sanayi_tuketim"),
                   MESKEN_TAHMİNLER = line_plot_generator(bolge_past_and_projection,"Mesken_tuketim"),
                   TİCARETHANE_TAHMİNLER = line_plot_generator(bolge_past_and_projection,"Ticarethane_tuketim"),
                   SULAMA_TAHMİNLER = line_plot_generator(bolge_past_and_projection,"Sulama_tuketim"),
                   AYDINLATMA_TAHMİNLER = line_plot_generator(bolge_past_and_projection,"Aydınlatma_tuketim"))


# counter for increasing the row indices
cnt <- 0

# insert the plots on to the specified worksheet of the specified workbook
for (i in 1:length(plot_array)) {

  # plot the output to the plotting pane
  plot(plot_array[[i]])

  # save the outputs locally
  ggsave(paste0(user_root_path,"/",
                config$Ana_Klasör_Yolu, "/",
                config$İl, "/",
                config$İlçe, "/",
                config$ELF$SONUÇLAR_klasör, "/",
                "Grafik Çıktıları", "/", 
         names(plot_array)[[i]], ".png"), height = 3.5, width = 7)


  # difference between rows of plots
  cnt <- cnt + 20

  graphics.off()
}

Sys.sleep(2)
print("Grafikler başarıyla kaydedildi.! ...")


##############################                                   ###############################################
##############################         WRITING TO EXCEL          ###############################################

print("Tahminler sonuç dosyasına yazılıyor...")
Sys.sleep(3)

# create a workbook object on R given the original data file
workbook <- loadWorkbook(excel_template_path)

# create style to be used for "bottom up 1", "bottom up 2" and "top down 1" models
my_style <- createStyle(fontName = "Calibri",
                        fontSize = 11,
                        valign = "center",
                        halign = "center",
                        numFmt = "#,##0")

#######################################################################

writeData(wb = workbook, sheet = "Bagımlı_Degisken_Tahminleri_1", x = bottom_up_model_minimum, startCol = 2, 
          startRow = last_known_year_nufus_index + 2, colNames = FALSE, rowNames = FALSE)
writeData(wb = workbook, sheet = "Bagımlı_Degisken_Tahminleri_2", x = bottom_up_model_dusuk, startCol = 2, 
          startRow = last_known_year_nufus_index + 2, colNames = FALSE, rowNames = FALSE)
writeData(wb = workbook, sheet = "Bagımlı_Degisken_Tahminleri_3", x = bottom_up_model_baz, startCol = 2, 
          startRow = last_known_year_nufus_index + 2, colNames = FALSE, rowNames = FALSE)
writeData(wb = workbook, sheet = "Bagımlı_Degisken_Tahminleri_4", x = bottom_up_model_yuksek, startCol = 2, 
          startRow = last_known_year_nufus_index + 2, colNames = FALSE, rowNames = FALSE)
writeData(wb = workbook, sheet = "Bagımlı_Degisken_Tahminleri_5", x = bottom_up_model_maksimum, startCol = 2, 
          startRow = last_known_year_nufus_index + 2, colNames = FALSE, rowNames = FALSE)


# Define the path to the SONUÇLAR directory to move the past results under the "Arşiv" section.
sonuclar_dir <- paste0(user_root_path,"/",
  config$Ana_Klasör_Yolu, "/",
  config$İl, "/",
  config$İlçe, "/",
  config$ELF$SONUÇLAR_klasör
)

# Define the path to the Arşiv subdirectory
arsiv_dir <- paste0(sonuclar_dir, "/Arşiv")

# Ensure the Arşiv directory exists (create it if it doesn't)
if (!dir.exists(arsiv_dir)) {
  dir.create(arsiv_dir, recursive = TRUE)
}

# List all files in the SONUÇLAR directory that start with "ELF_Tahmin_Sonuçları_"
existing_files <- list.files(sonuclar_dir, pattern = "^ELF_Tahmin_Sonuçları_.*", full.names = TRUE)

# Move each matching file to the Arşiv subdirectory
if (length(existing_files) > 0) {
  for (file in existing_files) {
    # Define the new path in the Arşiv directory
    new_path <- paste0(arsiv_dir, "/", basename(file))
    # Move the file
    file.rename(file, new_path)
  }
}


# Construct the full path for SONUÇLAR
sonuclar_file_name <- paste0("ELF_Tahmin_Sonuçları_",
                        str_split(timeDate::Sys.timeDate(FinCenter = "Istanbul") %>% as.character(), c(":"))[[1]][1],
                        "_",
                        str_split(timeDate::Sys.timeDate(FinCenter = "Istanbul") %>% as.character(), c(":"))[[1]][2],
                        "_",
                        str_split(timeDate::Sys.timeDate(FinCenter = "Istanbul") %>% as.character(), c(":"))[[1]][3],
                        ".xlsx"
)

# save the newly constructed Tahmin Sonuclar file.
saveWorkbook(workbook,
             file = paste0(user_root_path,"/",
                           config$Ana_Klasör_Yolu, "/",
                           config$İl, "/",
                           config$İlçe, "/",
                           config$ELF$SONUÇLAR_klasör, "/",
                           sonuclar_file_name),
             overwrite = TRUE)

print("Tahminler başarıyla sonuç dosyasına kaydedildi.!")
Sys.sleep(3)


# Update the SONUÇLAR key in the config object
config$ELF$SONUÇLAR_name <- sonuclar_file_name

# Write the updated config back to the original JSON file
jsonlite::write_json(config, args[1], pretty = TRUE, auto_unbox = TRUE)

