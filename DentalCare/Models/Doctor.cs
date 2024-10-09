using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Doctor
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public DateTime Firstdayofwork { get; set; }

    public bool Gender { get; set; }

    public string Falcutyid { get; set; } = null!;

    public string? Avatar { get; set; }

    public bool? Fired { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public string FacultyName { get; set; }

    public virtual Faculty Falcuty { get; set; } = null!;

    public virtual ICollection<Healthreport> Healthreports { get; set; } = new List<Healthreport>();

    public virtual ICollection<Medicalexamination> Medicalexaminations { get; set; } = new List<Medicalexamination>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
