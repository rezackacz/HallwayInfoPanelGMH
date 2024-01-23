using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HallwayInfoPanelGMH {
  public class BakaDataGatherer {

    List<Classroom> classooms_copy;

    string bakaServerURL;

    XElement? roomListXML = null;

    public BakaDataGatherer(List<Classroom> classrooms, string bakaServerURL) {
      bool error = false;
      this.bakaServerURL = bakaServerURL;
      try {
        roomListXML = XDocument.Load(bakaServerURL + "/b/common/rooms").Element("BakalariDataInterface");
      } catch (Exception e) {
        roomListXML = null; Console.Error.WriteLine(e.Message);
        error = true;
      }

      if (!error) {
        XElement? roomsXElement = roomListXML?.Elements().First(element => element.Name == "Rooms");

        this.bakaServerURL = bakaServerURL;

        foreach (Classroom classroom in classrooms) {

          classroom.bakaID = roomsXElement?.Descendants()?.First(room => (room?.Element("Abbrev")?.Value) == classroom.dispName)?.Element("ID")?.Value;
          classroom.roomURL = bakaServerURL + "/b/timetable/actual/room/" + classroom.bakaID;


          XElement timetable = XDocument.Load(classroom.roomURL)?.Element("BakalariDataInterface")?.Element("Cells");
          var dayIndex = ReturnCurrentDayIndex();
          var hourIndex = ReturnCurrentHourIndex(bakaServerURL + "/b/timetable/parameters");
          XElement HourNow;

          if (dayIndex != -1) {
            HourNow = timetable.Descendants("TimetableCell").FirstOrDefault(cell => { return ((int.Parse(cell.Element("DayIndex").Value) == dayIndex) && (int.Parse(cell.Element("HourIndex").Value) == hourIndex)); });
          } else {
            classroom.currentPeople = " "; classroom.subject = "Dnes se neučí."; classroom.currentTeacher = " ";
            continue;
          }
          if (HourNow == null) { classroom.currentPeople = " "; classroom.subject = "Neučí se."; classroom.currentTeacher = " "; } else {
            XElement atom = HourNow.Element("Atoms").Elements().First();
            classroom.currentPeople = atom.Element("Class").Element("Abbrev").Value;
            classroom.subject = atom.Element("Subject").Element("Abbrev").Value;
            classroom.currentTeacher = atom.Element("Teacher").Element("Abbrev").Value;
          }

        }
      } else {
 
        throw new Exception("Všechno je v piči.");
      }
      classooms_copy = classrooms;
    }






    public List<Classroom> getClassrooms() {
      List<Classroom> result = new();

      bool error = false;

      try {
        XDocument.Load(bakaServerURL + "/b/common/rooms");
      } catch (Exception e) {
        Console.Error.WriteLine(e.Message);
        error = true;
      }
      if (!error) {
        foreach (Classroom classroom in classooms_copy) {
          XElement timetable = null;

          try {
            timetable = XDocument.Load(classroom.roomURL).Element("BakalariDataInterface").Element("Cells");
          } catch (Exception e) {
            Console.Error.WriteLine(e.Message);
            break;
          }

          

          var dayIndex = ReturnCurrentDayIndex();
          var hourIndex = ReturnCurrentHourIndex(bakaServerURL + "/b/timetable/parameters");
          XElement HourNow;

          if (dayIndex != -1) {
            HourNow = timetable.Descendants("TimetableCell").FirstOrDefault(cell => { return ((int.Parse(cell.Element("DayIndex").Value) == dayIndex) && (int.Parse(cell.Element("HourIndex").Value) == hourIndex)); });
          } else {
            classroom.currentPeople = " "; classroom.subject = "Dnes se neučí."; classroom.currentTeacher = " ";
            continue;
          }
          if (HourNow == null) { classroom.currentPeople = " "; classroom.subject = "Neučí se."; classroom.currentTeacher = " "; } else {
            XElement atom = HourNow.Element("Atoms").Elements().First();
            classroom.currentPeople = atom.Element("Class").Element("Abbrev").Value;
            classroom.subject = atom.Element("Subject").Element("Abbrev").Value;
            classroom.currentTeacher = atom.Element("Teacher").Element("Abbrev").Value;
          }

        }
      } else {
        foreach (Classroom classroom in classooms_copy) {
          classroom.currentTeacher = " ";
          classroom.subject = "Chyba na straně systému Bakaláři.";
          classroom.currentPeople = " ";
        }
      }
      result = classooms_copy;
      return result;
    }


    public static int ReturnCurrentHourIndex(string xmlUrl) {
      XElement dataInterface = XDocument.Load(xmlUrl).Element("BakalariDataInterface");
      XElement hourDefinitions = dataInterface.Element("HourDefinitions");
      var defs = hourDefinitions.Descendants("HourDefinition");

      var hours = defs.Select(hodina => new { Caption = int.Parse(hodina.Element("Caption").Value), BeginTime = TimeSpan.Parse(hodina.Element("BeginTime").Value), EndTime = TimeSpan.Parse(hodina.Element("EndTime").Value) }).ToArray();

      TimeSpan now = DateTime.Now.TimeOfDay;

      for (int i = 0; i < hours.Length; i++) {
        if (now > TimeSpan.Parse("15:50")) return -1;

        if (now >= hours[i].BeginTime && now <= hours[i].EndTime) return hours[i].Caption + 2;

        if (now >= hours[i].EndTime && now <= hours[i + 1].BeginTime) return hours[i + 1].Caption + 2;
      }
      return -1;



    }

    public static int ReturnCurrentDayIndex() {
      switch (DateTime.Now.DayOfWeek) {
        case DayOfWeek.Sunday:
          return -1;
        case DayOfWeek.Monday:
          return 0;
        case DayOfWeek.Tuesday:
          return 1;
        case DayOfWeek.Wednesday:
          return 2;
        case DayOfWeek.Thursday:
          return 3;
        case DayOfWeek.Friday:
          return 4;
        case DayOfWeek.Saturday:
          return -1;
        default: return -1;
      }
    }
  }
}