using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Resolvers;
using System.Xml.XPath;

namespace HallwayInfoPanelGMH {
  public class CanteenMenuDownloader {
    /********************************
    věci které jsou relevantní v této classe jsou tady nahoře.
    (myslím těch 5 objektů Jidlo)
    co je který zač je snad jasné
    ********************************/

    Jidlo polevka;
    Jidlo jidlo1;
    Jidlo jidlo2;
    Jidlo jidlo3;
    Jidlo doplnek;


    private string URL;
    private XDocument? xml = null;
    private XElement? today;

    public string PageTitle;
    public List<Jidlo> dnesniJidla = new List<Jidlo>();

    public CanteenMenuDownloader(string URL) {
      this.URL = URL;

      download();

    }

    // Filters the given XElement based on the specified food type and returns the filtered result.
    private static Jidlo vyfiltruj (XElement today, string druh) {
      Jidlo vysledek = new Jidlo();
      XElement? xElement = today.Elements("jidlo").FirstOrDefault(food => food?.Attribute("druh")?.Value == druh);
      if (xElement == null) { throw new FoodNotAvailableException("Jidlo " + druh + "se nepodarilo ziskat"); } else {
        vysledek.druh = druh;
        vysledek.nazev = xElement.Attribute("nazev").Value;
        return vysledek;
      }
    }



    public class Jidlo {
      public int ID;
      public string nazev { get; set; }
      public string druh { get; set; }

      public Jidlo() {
        nazev = "default";
        druh = "default";
      }
      public Jidlo(string nazev, string druh) {
        this.nazev = nazev;
        this.druh = druh;
      }



    }

    public void update() {
      download();
    }

    private void download() {
      try { this.xml = XDocument.Load(URL); }
      catch(System.Net.Http.HttpRequestException e) { this.xml = null; Console.Error.WriteLine(e.Message); }
      XElement? xNjidelnicek = (XElement)xml?.FirstNode;
      this.today = (XElement)xNjidelnicek?.FirstNode;

      string date = DateTime.Now.ToString("dd-MM-yyyy");

      if (today == null || !today.Attribute("datum").Value.Equals(date)) {
        PageTitle = "Dnešní jídelníček neexistuje v databázi.";
        Console.Error.WriteLine("Error: today's menu doesn't exist in database.");
        this.polevka = new Jidlo("Není k dispozici", "Polévka");
        this.jidlo1 = new Jidlo("Není k dispozici", "Oběd 1S");
        this.jidlo2 = new Jidlo("Není k dispozici", "Oběd 2S");
        this.jidlo3 = new Jidlo("Není k dispozici", "Oběd 3S");
        this.doplnek = new Jidlo("Není k dispozici", "Doplněk");
      } else {
        this.PageTitle = "Dnešní jídelníček";

        Jidlo jidloTemp = vyfiltruj(today, "Polévka ");
        if (jidloTemp == null) this.polevka = new Jidlo("Není k dispozici", "Polévka"); else this.polevka = jidloTemp;

        jidloTemp = vyfiltruj(today, "Oběd 1S ");
        if (jidloTemp == null) this.jidlo1 = new Jidlo("Není k dispozici", "Oběd 1S"); else this.jidlo1 = jidloTemp;

        jidloTemp = vyfiltruj(today, "Oběd 2S ");
        if (jidloTemp == null) this.jidlo2 = new Jidlo("Není k dispozici", "Oběd 2S"); else this.jidlo2 = jidloTemp;

        jidloTemp = vyfiltruj(today, "Oběd 3S ");
        if (jidloTemp == null) this.jidlo3 = new Jidlo("Není k dispozici", "Oběd 3S"); else this.jidlo3 = jidloTemp;

        jidloTemp = vyfiltruj(today, "Doplněk ");
        if (jidloTemp == null) this.doplnek = new Jidlo("Není k dispozici", "Doplněk"); else this.doplnek = jidloTemp;
      }
    }


    public string toDivTableString() {
      string result;

      result = "<div class=\"table-container\">";
      result += toDivRowString(polevka);
      result += toDivRowString(jidlo1);
      result += toDivRowString(jidlo2);
      result += toDivRowString(jidlo3);
      result += toDivRowString(doplnek);
      result += "</div>";


      return result;
    }

    internal string toDivRowString(Jidlo food) {
      string result;

      result = "<div class=\"table-row\">";
      result += "<div class=\"meal-name\"><b>" + food.druh + "</b></div>";
      result += "<div class=\"meal-description\">" + food.nazev + "</div>";
      result += "</div>";
      return result;
    }

  }






  public class FoodNotAvailableException : Exception {
    public FoodNotAvailableException() { }
    public FoodNotAvailableException(string message) : base(message) { }
    public FoodNotAvailableException(string message, Exception inner) : base(message, inner) { }


  }
}
