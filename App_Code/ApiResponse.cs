using System;
using System.Collections.Generic;

public class ApiResponse
{
    public string version { get; set; }
    public StudentData data { get; set; }
}

public class StudentData
{

    public string bsebUniqueID { get; set; }   
    public DateTime? dob { get; set; }         

    public string rollCode { get; set; }
    public string rollNo { get; set; }
    public string regno { get; set; }
    public string sn { get; set; }
    public string fn { get; set; }
    public string mn { get; set; }
    public string clgname { get; set; }
    public string faculty { get; set; }
    public string totalAggMarks { get; set; }
    public string totalAggWords { get; set; }
    public string division { get; set; }

    public List<SubjectResult> subjectResults { get; set; }
}

public class SubjectResult
{
    public string sub { get; set; }

    public int? maxMark { get; set; }
    public int? passMark { get; set; }

    public string theory { get; set; }
    public string oB_PR { get; set; }

    public string grC_THO { get; set; }
    public string grC_PR { get; set; }

    public string cceMarks { get; set; }

    public string totSub { get; set; }

    public string subjectGroupName { get; set; }

}