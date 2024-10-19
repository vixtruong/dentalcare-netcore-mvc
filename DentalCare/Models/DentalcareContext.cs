using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DentalCare.Models;

public partial class DentalcareContext : DbContext
{
    public DentalcareContext()
    {
    }

    public DentalcareContext(DbContextOptions<DentalcareContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Equipment> Equipments { get; set; }

    public virtual DbSet<Equipmentdetail> Equipmentdetails { get; set; }

    public virtual DbSet<Equipmenttype> Equipmenttypes { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Healthreport> Healthreports { get; set; }

    public virtual DbSet<Healthreportdetail> Healthreportdetails { get; set; }

    public virtual DbSet<Medicalexamination> Medicalexaminations { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<Medicinetype> Medicinetypes { get; set; }

    public virtual DbSet<Nurse> Nurses { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<Prescriptiondetail> Prescriptiondetails { get; set; }

    public virtual DbSet<Receptionist> Receptionists { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Techdetail> Techdetails { get; set; }

    public virtual DbSet<Technique> Techniques { get; set; }

    public virtual DbSet<Techposition> Techpositions { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DBDefault"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Phone).HasName("PK__ACCOUNT__D4FA0A27C26550E7");

            entity.ToTable("ACCOUNT");

            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PHONE");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Role)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ROLE");
            entity.Property(e => e.DoctorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DOCTORID");
            entity.Property(e => e.ReceptionistId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RECEPTIONISTID");
            entity.HasOne(d => d.Doctor)
                .WithMany(p => p.Accounts)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK_ACCOUNT_DOCTOR");

            entity.HasOne(d => d.Receptionist)
                .WithMany(p => p.Accounts)
                .HasForeignKey(d => d.ReceptionistId)
                .HasConstraintName("FK_Account_Receptionist");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__APPOINTM__3214EC27E8713B98");

            entity.ToTable("APPOINTMENT");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Customerid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUSTOMERID");
            entity.Property(e => e.Date)
                .HasColumnType("smalldatetime")
                .HasColumnName("DATE");
            entity.Property(e => e.Doctorid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DOCTORID");
            entity.Property(e => e.Time)
                .HasColumnName("TIME");

            entity.Property(e => e.Demand)
                .HasMaxLength(200)
                .HasColumnName("DEMAND");

            entity.HasOne(d => d.Customer).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_APPOINTMENT_CUSTOMER");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Doctorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_APPOINTMENT_DOCTOR");
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BILL__3214EC27E76AE606");

            entity.ToTable("BILL");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasColumnType("smalldatetime")
                .HasColumnName("DATE");
            entity.Property(e => e.Discount).HasColumnName("DISCOUNT");
            entity.Property(e => e.Finaltotal).HasColumnName("FINALTOTAL");
            entity.Property(e => e.Medicalexaminationid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MEDICALEXAMINATIONID");
            entity.Property(e => e.Payment).HasColumnName("PAYMENT");
            entity.Property(e => e.Receptionistid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RECEPTIONISTID");
            entity.Property(e => e.Total).HasColumnName("TOTAL");

            entity.HasOne(d => d.Medicalexamination).WithMany(p => p.Bills)
                .HasForeignKey(d => d.Medicalexaminationid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BILL_MEDICALEXAMINATION");

            entity.HasOne(d => d.Receptionist).WithMany(p => p.Bills)
                .HasForeignKey(d => d.Receptionistid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BILL_RECEPTIONIST");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CUSTOMER__3214EC27EFA61434");

            entity.ToTable("CUSTOMER");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Birthday)
                .HasColumnType("smalldatetime")
                .HasColumnName("BIRTHDAY");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PHONE");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DOCTOR__3214EC2709DA62B5");

            entity.ToTable("DOCTOR");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Avatar)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("AVATAR");
            entity.Property(e => e.Birthday)
                .HasColumnType("smalldatetime")
                .HasColumnName("BIRTHDAY");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Facultyid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FALCUTYID");
            entity.Property(e => e.Firstdayofwork)
                .HasColumnType("smalldatetime")
                .HasColumnName("FIRSTDAYOFWORK");
            entity.Property(e => e.Gender).HasColumnName("GENDER");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PHONE");
            entity.Property(e => e.Fired).HasColumnName("FIRED");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.Facultyid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DOCTOR_FALCUTY");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EQUIPMEN__3214EC2792C5AEC7");

            entity.ToTable("EQUIPMENT");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Equipmenttypeid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EQUIPMENTTYPEID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Price).HasColumnName("PRICE");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
            entity.Property(e => e.Unit)
                .HasMaxLength(10)
                .HasColumnName("UNIT");

            entity.HasOne(d => d.Equipmenttype).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.Equipmenttypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EQUIPMENT_EQUIPMENTTYPE");
        });

        modelBuilder.Entity<Equipmentdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EQUIPMEN__3214EC27EF1F9229");

            entity.ToTable("EQUIPMENTDETAIL");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasColumnType("smalldatetime")
                .HasColumnName("DATE");
            entity.Property(e => e.Equipmentid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EQUIPMENTID");
            entity.Property(e => e.Medicalexaminationid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MEDICALEXAMINATIONID");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

            entity.HasOne(d => d.Equipment).WithMany(p => p.Equipmentdetails)
                .HasForeignKey(d => d.Equipmentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EQUIPMENTDETAIL_EQUIPMENT");

            entity.HasOne(d => d.Medicalexamination).WithMany(p => p.Equipmentdetails)
                .HasForeignKey(d => d.Medicalexaminationid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EQUIPMENTDETAIL_MEDICALEXAMINATION");
        });

        modelBuilder.Entity<Equipmenttype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EQUIPMEN__3214EC2711336FA0");

            entity.ToTable("EQUIPMENTTYPE");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FALCUTY__3214EC27FD2340EC");

            entity.ToTable("FALCUTY");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Healthreport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HEALTHRE__3214EC27AE22AE18");

            entity.ToTable("HEALTHREPORT");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasColumnType("smalldatetime")
                .HasColumnName("DATE");
            entity.Property(e => e.Doctorid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DOCTORID");
            entity.Property(e => e.Frequency)
                .ValueGeneratedOnAdd()
                .HasColumnName("FREQUENCY");
            entity.Property(e => e.Status)
                .HasMaxLength(500)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Healthreports)
                .HasForeignKey(d => d.Doctorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HEALTHREPORT_DOCTOR");
        });

        modelBuilder.Entity<Healthreportdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HEALTHRE__3214EC278E2845CF");

            entity.ToTable("HEALTHREPORTDETAIL");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Customerid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUSTOMERID");
            entity.Property(e => e.Healthreportid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("HEALTHREPORTID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Healthreportdetails)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HEALTHREPORTDETAIL_CUSTOMER");

            entity.HasOne(d => d.Healthreport).WithMany(p => p.Healthreportdetails)
                .HasForeignKey(d => d.Healthreportid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HEALTHREPORTDETAIL_HEALTHREPORT");
        });

        modelBuilder.Entity<Medicalexamination>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MEDICALE__3214EC27514E986F");

            entity.ToTable("MEDICALEXAMINATION");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Customerid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUSTOMERID");
            entity.Property(e => e.Date)
                .HasColumnType("smalldatetime")
                .HasColumnName("DATE");
            entity.Property(e => e.Doctorid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DOCTORID");
            entity.Property(e => e.Status)
                .HasMaxLength(500)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Customer).WithMany(p => p.Medicalexaminations)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEDICALDETAIL_CUSTOMER");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Medicalexaminations)
                .HasForeignKey(d => d.Doctorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEDICALDETAIL_DOCTOR");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MEDICINE__3214EC277CFFD592");

            entity.ToTable("MEDICINE");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Medicinetypeid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MEDICINETYPEID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Price).HasColumnName("PRICE");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
            entity.Property(e => e.Unit)
                .HasMaxLength(10)
                .HasColumnName("UNIT");

            entity.HasOne(d => d.Medicinetype).WithMany(p => p.Medicines)
                .HasForeignKey(d => d.Medicinetypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEDICINE_MEDICINETYPE");
        });

        modelBuilder.Entity<Medicinetype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MEDICINE__3214EC270623118A");

            entity.ToTable("MEDICINETYPE");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Nurse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NURSE__3214EC27DDEF86E1");

            entity.ToTable("NURSE");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Avatar)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("AVATAR");
            entity.Property(e => e.Birthday)
                .HasColumnType("smalldatetime")
                .HasColumnName("BIRTHDAY");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Firstdayofwork)
                .HasColumnType("smalldatetime")
                .HasColumnName("FIRSTDAYOFWORK");
            entity.Property(e => e.Gender).HasColumnName("GENDER");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PHONE");
            entity.Property(e => e.Fired).HasColumnName("FIRED");
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OWNER__3214EC27F8690CDD");

            entity.ToTable("OWNER");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PHONE");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRESCRIP__3214EC2737AA1E20");

            entity.ToTable("PRESCRIPTION");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasColumnType("smalldatetime")
                .HasColumnName("DATE");
            entity.Property(e => e.Doctorid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DOCTORID");
            entity.Property(e => e.Medicalexaminationid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MEDICALEXAMINATIONID");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.Doctorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PRESCRIPTION_DOCTOR");

            entity.HasOne(d => d.Medicalexamination).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.Medicalexaminationid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PRESCRIPTION_MEDICALEXAMINATION");
        });

        modelBuilder.Entity<Prescriptiondetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRESCRIP__3214EC278957EF0A");

            entity.ToTable("PRESCRIPTIONDETAIL");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Medicineid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MEDICINEID");
            entity.Property(e => e.Prescriptionid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PRESCRIPTIONID");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

            entity.HasOne(d => d.Medicine).WithMany(p => p.Prescriptiondetails)
                .HasForeignKey(d => d.Medicineid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PRESCRIPTIONDETAIL_MEDICINEID");

            entity.HasOne(d => d.Prescription).WithMany(p => p.Prescriptiondetails)
                .HasForeignKey(d => d.Prescriptionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PRESCRIPTIONDETAIL_PRESCRIPTION");
        });

        modelBuilder.Entity<Receptionist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RECEPTIO__3214EC27D1CB338C");

            entity.ToTable("RECEPTIONIST");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Avatar)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("AVATAR");
            entity.Property(e => e.Birthday)
                .HasColumnType("smalldatetime")
                .HasColumnName("BIRTHDAY");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Firstdayofwork)
                .HasColumnType("smalldatetime")
                .HasColumnName("FIRSTDAYOFWORK");
            entity.Property(e => e.Gender).HasColumnName("GENDER");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PHONE");
            entity.Property(e => e.Fired).HasColumnName("FIRED");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SHIFT__3214EC27766E17A6");

            entity.ToTable("SHIFT");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasColumnType("smalldatetime")
                .HasColumnName("DATE");
            entity.Property(e => e.Doctorid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DOCTORID");
            entity.Property(e => e.Nurseid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("NURSEID");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Shifts)
                .HasForeignKey(d => d.Doctorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SHIFT_DOCTOR");

            entity.HasOne(d => d.Nurse).WithMany(p => p.Shifts)
                .HasForeignKey(d => d.Nurseid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SHIFT_NURSE");
        });

        modelBuilder.Entity<Techdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TECHDETA__3214EC27AF387AEA");

            entity.ToTable("TECHDETAIL");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Medicalexaminationid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MEDICALEXAMINATIONID");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
            entity.Property(e => e.Techpositionid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TECHPOSITIONID");

            entity.HasOne(d => d.Medicalexamination).WithMany(p => p.Techdetails)
                .HasForeignKey(d => d.Medicalexaminationid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TECHDETAIL_MEDICALEXAMINATION");

            entity.HasOne(d => d.Techposition).WithMany(p => p.Techdetails)
                .HasForeignKey(d => d.Techpositionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TECHDETAIL_TECHPOSITION");
        });

        modelBuilder.Entity<Technique>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TECHNIQU__3214EC27AA59D961");

            entity.ToTable("TECHNIQUE");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Techposition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TECHPOSI__3214EC2724A1C9DC");

            entity.ToTable("TECHPOSITION");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Price).HasColumnName("PRICE");
            entity.Property(e => e.Techniqueid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TECHNIQUEID");
            entity.Property(e => e.Unit)
                .HasMaxLength(10)
                .HasColumnName("UNIT");

            entity.HasOne(d => d.Technique).WithMany(p => p.Techpositions)
                .HasForeignKey(d => d.Techniqueid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TECHPOSITION_TECHNIQUE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
