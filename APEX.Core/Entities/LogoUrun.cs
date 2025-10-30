namespace APEX.Core.Entities
{
    public class LogoUrun
    {
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public string Barkod { get; set; } = string.Empty;
        public string Birim { get; set; } = string.Empty;
        public decimal BirimFiyat { get; set; }
        public decimal KdvOrani { get; set; }
        public string KategoriKodu { get; set; } = string.Empty;
        public string GrupKodu { get; set; } = string.Empty;
        public decimal MevcutStok { get; set; }
        public decimal MinimumStok { get; set; }
        public decimal MaksimumStok { get; set; }
        public bool Aktif { get; set; } = true;
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public string Aciklama { get; set; } = string.Empty;
        public string UreticiKodu { get; set; } = string.Empty;
        public decimal Agirlik { get; set; }
        public string RenkKodu { get; set; } = string.Empty;
        public string BedenKodu { get; set; } = string.Empty;
        public List<string> AlternatifBarkodlar { get; set; } = new List<string>();
    }

    public class LogoUrunKategori
    {
        public string KategoriKodu { get; set; } = string.Empty;
        public string KategoriAdi { get; set; } = string.Empty;
        public string UstKategoriKodu { get; set; } = string.Empty;
        public bool Aktif { get; set; } = true;
        public int Seviye { get; set; }
    }
}