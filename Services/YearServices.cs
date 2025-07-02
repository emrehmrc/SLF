using System;

namespace SLF.Services
{
    /// <summary>
    /// Singleton servis sınıfı ile SLF başlangıç ve bitiş yıllarını merkezi olarak yönetme
    /// </summary>
    public class YearService
    {
        private static YearService _instance;
        private static readonly object _lock = new object();

        // Yıl değişkenleri
        private int _slfStartYear;
        private int _slfEndYear;

        /// <summary>
        /// YearService singleton instance'ını döndürür
        /// </summary>
        public static YearService GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new YearService();
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        /// Private constructor - Singleton pattern için
        /// </summary>
        private YearService()
        {
            // Default değerler
            _slfStartYear = 0;
            _slfEndYear = 0;
        }

        /// <summary>
        /// SLF başlangıç yılı
        /// </summary>
        public int slfStartYear
        {
            get { return _slfStartYear; }
            set
            {
                _slfStartYear = value;
                // Değiştiği zaman olay tetiklenebilir
                OnYearChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// SLF bitiş yılı
        /// </summary>
        public int slfEndYear
        {
            get { return _slfEndYear; }
            set
            {
                _slfEndYear = value;
                // Değiştiği zaman olay tetiklenebilir
                OnYearChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Son yıl (başlangıç yılından bir önceki)
        /// </summary>
        public int LastYear
        {
            get { return slfStartYear; }
        }

        /// <summary>
        /// İki önceki yıl (başlangıç yılından iki önceki)
        /// </summary>
        public int PenultimateYear
        {
            get { return slfStartYear - 1; }
        }

        /// <summary>
        /// Ufuk yılı (başlangıç yılı)
        /// </summary>
        public int HorizonYear
        {
            get { return slfStartYear+1; }
        }

        /// <summary>
        /// Yıl değişikliğinde tetiklenen olay
        /// </summary>
        public event EventHandler OnYearChanged;

        /// <summary>
        /// Tüm yıl değerlerini tek seferde belirleme
        /// </summary>
        public void SetYears(int startYear, int endYear)
        {
            _slfStartYear = startYear;
            _slfEndYear = endYear;
            OnYearChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Yıl değerlerinin geçerli olup olmadığını kontrol eder
        /// </summary>
        public bool AreYearsValid()
        {
            return slfStartYear > 0 && slfEndYear > 0 && slfEndYear >= slfStartYear;
        }
    }
}