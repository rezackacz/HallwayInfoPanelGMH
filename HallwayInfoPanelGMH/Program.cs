﻿using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;


namespace HallwayInfoPanelGMH {
  internal class Program {

    static string bakaServerURL;
    static string stravaCzURL;

    static BakaDataGatherer? timeTableDownloader = null;

    static string htmlOutFolder;

    static void Main(string[] args) {

      if (args.Length < 1) { Console.WriteLine("Použití: HallwayInfoPanelGMH (cesta k config souboru)"); return; }

      XElement? config = XDocument.Load(args[0]).Element("InfopanelConfiguration");
      if (config == null) { throw new FileNotFoundException("konfiguracni soubor se nepovedlo nacist."); }

      stravaCzURL = config.Element("StravaURL").Value;
      htmlOutFolder = config.Element("HTMLFileFolder").Value;

      var jidloStahovac = new CanteenMenuDownloader(stravaCzURL);
      var foodRenderer = new HTMLRenderer(jidloStahovac, htmlOutFolder + "/jidelna.html");

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

        Gatherers.Add(new(ListClsGroup, bakaServerURL));
        Renderers.Add(new(ListClsGroup, htmlOutFolder + "classroomGroup" + XElementClsGroup.Element("index").Value + ".html"));

        ListClsGroups.Add(ListClsGroup);
      }



      while (true) {
        jidloStahovac.update();
        foodRenderer.RenderAndSaveAsHTML(jidloStahovac);

        foreach (var gatherer in Gatherers) {

        }
        //  classroomRenderer.RenderAndSaveAsHTML(classroom_downloader.getClassrooms());
      }




    }
  }
}
