﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallwayInfoPanelGMH {
  public class Classroom {

    public int id { get; }
    public string dispName { get; }
    public string? currentPeople { get; set; }
    public string? subject { get; set; }
    public string? currentTeacher { get; set; }
    public string? bakaID { get; set; } 
    public string? roomURL { get; set; }

    public Classroom(int id, string dispName) {
      this.id = id;
      this.dispName = dispName;

    }

    public Classroom(int id, string dispName, string currentPeople, string subject, string currentTeacher) {
      this.id = id;
      this.dispName = dispName;
      this.currentPeople = currentPeople;
      this.subject = subject;
      this.currentTeacher = currentTeacher;


    }


    public string toDivRowString() {
      string result;
      result = "<div class=\"table-row\">";
      result += "<div><b>" + this.dispName + "</b></div>";
      result += "<div>" + this.currentPeople + "</div>";
      result += "<div>" + this.subject + "</div>";
      result += "<div>" + this.currentTeacher + "</div>";

      return result;
    }

  }
}
