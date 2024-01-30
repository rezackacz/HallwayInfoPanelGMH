using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallwayInfoPanelGMH {
  class HTMLRenderer {

    private const string htmlBeginning = "<!DOCTYPE html><html><head><title>GMH Infopanel</title><meta charset=\"UTF-16\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><style>";
    private const string CSS = "@font-face{ font-family: 7segment; src: url(7segment.ttf);} body { font-family: sans-serif; font-size: 4vw;} .table-container { display: table; flex-direction: column; align-items: center; margin-top: 5px; width: 100%} .table-row { display: table-row; justify-content: space-between; width: 100%; border-bottom: 1px solid #ccc; padding: 10px 0;} .title { text-align:center; font-weight:bold} .meal-description { text-align: right; flex-grow:1; max-width: 80%; } .subject {text-align:center; min-width: 6%;} #hodiny { position: absolute; bottom: 0; right: 0; font-size: 10vw; font-family: \"7-Segment\"; font-weight: bold;} .table-cell { display: table-cell; border-bottom: 2px solid } .menu-container { display: flex; flex-direction: column; align-items: center; margin-top: 5px;} .menu-row { display: flex; justify-content: space-between; width: 100%; border-bottom: 1px solid #ccc; padding: 10px 0;}";
    private const string htmlMiddle = "</style> <script> function start(){ const today = new Date(); let h = today.getHours(); let m = today.getMinutes(); let s = today.getSeconds(); h = addZero(h); m = addZero(m); s = addZero(s); document.getElementById('hodiny').innerHTML = h + \":\" + m + \":\" + s; setTimeout(start, 100); } function addZero(i) { if (i < 10) {i = \"0\" + i}; return i; } </script>  </head><body onLoad=\"start()\"><div id=\"hodiny\"></div>";
    private string body;
    private const string htmlEnding = "</body></html>";

    private string pageTitle;
    public string htmlFileName { get; }

    public enum DisplayTypes {
      INFO, CLASSROOM_LIST, CANTEEN_MENU, WARNING, WEATHER, JOKE
    }

    public DisplayTypes type;


    // constructors start

    public HTMLRenderer(DisplayTypes typ, string pageTitle, string body, string htmlFileName) {
      this.pageTitle = pageTitle;
      this.body = body;
      this.htmlFileName = htmlFileName;
    }


    public HTMLRenderer(List<Classroom> classrooms, string htmlFileName, string pageTitle) {

      this.type = DisplayTypes.CLASSROOM_LIST;
      this.htmlFileName = htmlFileName;

      this.pageTitle = pageTitle;

      this.RenderAndSaveAsHTML(classrooms);

    }

    public HTMLRenderer(CanteenMenuDownloader canteenMenuDownloader, string htmlFileName, string pageTitle) {
      this.type = DisplayTypes.CANTEEN_MENU;
      this.htmlFileName= htmlFileName;

      this.pageTitle = pageTitle;

      this.RenderAndSaveAsHTML(canteenMenuDownloader);
    }

    //constructors end

    public void RenderAndSaveAsHTML(List<Classroom> classrooms) {
      if (this.type != DisplayTypes.CLASSROOM_LIST) { Console.Error.WriteLine("předán nesprávný argument"); return; }

      body = "<div class=\"title\">"+pageTitle+"</div><div class=\"table-container\">";

      List<string> rowsList = new List<string>();
      foreach (var classroom in classrooms) {
        rowsList.Add(classroom.toDivRowString());
      }
      foreach (var divRow in rowsList) {
        body += divRow;
        body += "</div>";
      }
      body += "</div>";


      StringBuilder sb = new StringBuilder();
      sb.Append(htmlBeginning);
      sb.Append(CSS);
      sb.Append(htmlMiddle);
      sb.Append(body);
      sb.Append(htmlEnding);

      File.WriteAllText(htmlFileName, sb.ToString());
    }

    public void RenderAndSaveAsHTML(CanteenMenuDownloader canteenMenuDownloader) {
      if(this.type != DisplayTypes.CANTEEN_MENU) { Console.Error.WriteLine("předán nesprávný argument"); return; }

      body = "<div class=\"title\">" + pageTitle + "</div>";
      body += canteenMenuDownloader.toDivTableString();

      StringBuilder sb = new StringBuilder();
      sb.Append(htmlBeginning);
      sb.Append(CSS);
      sb.Append(htmlMiddle);
      sb.Append(body);
      sb.Append(htmlEnding);
      File.WriteAllText(htmlFileName, sb.ToString());

    }
  }
}
