using System.Xml.Linq;

namespace Aoun.DAL.Entities.Auth;
public class CharityDocument {
    //public int Id { get; set; }

    //// اسم الملف الأصلي (عشان يظهر في الواجهة)
    //public string DocumentName { get; set; } = string.Empty;
    //// نوع المستند (TaxCard - License - NationalId ...)
    //public string DocumentType { get; set; } = string.Empty;
    ////  public bool IsDeleted { get; set; } = false;

    //// مسار الملف على السيرفر
    //public string FilePath { get; set; } = string.Empty;

    //public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    //public int CharityProfileId { get; set; }
    //public CharityProfile? CharityProfile { get; set; }



    public int Id { get; set; }

    public int CharityProfileId { get; set; }
    public CharityProfile CharityProfile { get; set; } = null!;

    // اسم الملف بعد التخزين
    public string FileName { get; set; } = string.Empty;

    // مسار الملف على السيرفر
    public string FilePath { get; set; } = string.Empty;

    // نوع المستند (هنستخدمه نعرف ده ايه)
    public DocumentType DocumentType { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;


   
}













