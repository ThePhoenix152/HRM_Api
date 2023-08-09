using System;
using System.Collections.Generic;

namespace BussinessObjects.Models;

public partial class Candidate
{
    public int CandidateId { get; set; }

    public string? ImageFile { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime? Dob { get; set; }

    public bool? Gender { get; set; }

    public string? Address { get; set; }

    public string Department { get; set; } = null!;

    public int? ExpectedSalary { get; set; }

    public string? ResumeFile { get; set; }

    public DateTime ApplyDate { get; set; }

    public string? Result { get; set; }
}
