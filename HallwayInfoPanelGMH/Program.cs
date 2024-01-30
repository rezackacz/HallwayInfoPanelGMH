namespace HallwayInfoPanelGMH {
  internal class Program {

    static string bakaServerURL;
    static string stravaCzURL;

    static BakaDataGatherer? timeTableDownloader = null;

    static string htmlOutFolder;

    static void Main(string[] args) {
    START:


      if (args.Length < 1) { Console.WriteLine("Použití: HallwayInfoPanelGMH (cesta k config souboru)"); return; }

      XElement? config = XDocument.Load(args[0]).Element("InfopanelConfiguration");
      if (config == null) { throw new FileNotFoundException("konfiguracni soubor se nepovedlo nacist."); }

      stravaCzURL = config.Element("StravaURL").Value;
      htmlOutFolder = config.Element("HTMLFileFolder").Value;

      var jidloStahovac = new CanteenMenuDownloader(stravaCzURL);
      var foodRenderer = new HTMLRenderer(jidloStahovac, htmlOutFolder + "/jidelna.html", "Jídelníček");

      bakaServerURL = config.Element("BakalariURL").Value;

      var XElementClsGroups = config.Element("ClassroomGroups");
      var ListClsGroups = new List<List<Classroom>>();
      var Gatherers = new List<BakaDataGatherer>();
      var Renderers = new List<HTMLRenderer>();

      foreach (var XElementClsGroup in XElementClsGroups.Descendants("ClassroomGroup")) {
        var ListClsGroup = new List<Classroom>();
        foreach (var XElementCls in XElementClsGroup.Descendants("Classroom")) {
          Classroom classroom = new(int.Parse(XElementCls.Element("index").Value), XElementCls.Element("Abbrev").Value);

          ListClsGroup.Add(classroom);
        }

        try {
          Gatherers.Add(new(ListClsGroup, bakaServerURL));
        } catch (Exception ex) {
          Console.WriteLine(ex.Message);
          goto START;
        }

        Renderers.Add(new(ListClsGroup, htmlOutFolder + "classroomGroup" + XElementClsGroup.Element("index").Value + ".html", "Aktuální rozvrh"));

        ListClsGroups.Add(ListClsGroup);
      }

      Console.WriteLine("First render finished at:" + DateTime.Now);

      while (true) {
        System.Threading.Thread.Sleep(500);

        jidloStahovac.update();
        foodRenderer.RenderAndSaveAsHTML(jidloStahovac);

        foreach (var gatherer in Gatherers) {
          Renderers[Gatherers.IndexOf(gatherer)].RenderAndSaveAsHTML(gatherer.getClassrooms());
        }

        Console.WriteLine("Render finished at:" + DateTime.Now);


      }




    }



  }
}
