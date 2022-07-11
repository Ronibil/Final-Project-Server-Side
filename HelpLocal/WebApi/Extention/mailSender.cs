using DATA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using WebApi.DTO;

namespace WebApi.Extention
{
    public static class mailSender
    {
        static string ProjMail = "help.me.student.ruppin@gmail.com";
        static string Password = "nihovjusniqpmbdv";
        static string Host = "smtp.gmail.com";
        static int Port = 587;
        static AppDbContext db = new AppDbContext();

        public static void sendEmailVerify(tblRequestToJoin user, string requestStatus)
        {
            SmtpClient client = new SmtpClient(Host, Port)
            {
                Credentials = new NetworkCredential(ProjMail, Password),
                EnableSsl = true
            };

            if (requestStatus.ToString() == "approved")
            {
                string userPassword = db.tblStudent.SingleOrDefault(s => s.StudentId == user.StudentId).Password;
                string subject = "מצב בקשתך להצטרפות לאפליקציית - HelpMeStudent";
                string body = $"שלום רב, {user.FullName}, אנו מעדכנים כי בקשתך להצטרפות לאפליקציית אושרהHelpMeStudeny . שם המשתמש שלך: {user.Email} הסיסמא לכניסה: {userPassword} ";
                client.Send(ProjMail, user.Email, subject, body);
            }
            else
            {
                string subject = "מצב בקשתך להצטרפות לאפליקציית - HelpMeStudent";
                string body = $"שלום רב, אנו מעדכנים כי בקשתך להצטרופות לאפליקציית HelpMeStudent נדחתה בגלל שלא נמצאה התאמה במערכת המרכז האקדמי רופין";
                client.Send(ProjMail, user.Email, subject, body);
            };
        }

        public static void sendEmailStudentUpdateOnCanceledClass(tblStudent student, tblClass c)
        {
            SmtpClient client = new SmtpClient(Host, Port)
            {
                Credentials = new NetworkCredential(ProjMail, Password),
                EnableSsl = true
            };

            string subject = $"{c.ClassDate} עדכון בנושא שיעור שאתה רשום בתאריך  - HelpMeStudent";
            string body = $"שלום רב, {student.FullName},אנו מעדכנים כי השיעור ב-{c.ClassName} בתאריך זה בוטל מסיבות אישיות של הסופר סטודנט HelpMeStudeny";
            client.Send(ProjMail, student.Email, subject, body);
        }

        public static void sendEmailToSuperStudentUpdateRegistered(tblSuperStudent superS, tblClass c, tblStudent student)
        {
            SmtpClient client = new SmtpClient(Host, Port)
            {
                Credentials = new NetworkCredential(ProjMail, Password),
                EnableSsl = true
            };

            string subject = $" עדכון בנושא רישום לשיעור {c.ClassDate},{c.ClassName} - HelpMeStudent";
            string body = $"שלום רב, {superS.tblStudent.FullName}, אנו מעדכנים כי נוסף סטודנט חדש לשיעור." +
                $"פרטי הסטודנט: {student.FullName}," +
                $"מספר פלאפון: {student.Phone} " +
                $"מייל: {student.Email}";
            client.Send(ProjMail, superS.tblStudent.Email, subject, body);
        }

        public static void sendEmailToSuperStudentUpdateDelete(string superName,string Email ,tblStudent student, tblClass c)
        {
            SmtpClient client = new SmtpClient(Host, Port)
            {
                Credentials = new NetworkCredential(ProjMail, Password),
                EnableSsl = true
            };

            string subject = $" עדכון בנושא אי הגעה לשיעור {c.ClassDate},{c.ClassName} - HelpMeStudent";
            string body = $"שלום רב, {superName}, אנו מעדכנים כי הסטודנט לא יגיע לשיעור." +
                $"פרטי הסטודנט: {student.FullName}," +
                $"מספר פלאפון: {student.Phone} " +
                $"מייל: {student.Email}";
            client.Send(ProjMail, Email, subject, body);
        }
        public static void sendNewPasswordToUser (string name, string password, string email)
        {
            SmtpClient client = new SmtpClient(Host, Port)
            {
                Credentials = new NetworkCredential(ProjMail, Password),
                EnableSsl = true
            };
            string subject = "HelpMeStudent - הפקת סיסמא חדשה";
            string body = "שלום רב, " + name + " :) \r\nאנו שמחים לעדכן אותך כי הופקה סיסמא חדשה עבור החשבון שלך במערכת שלנו. \r\nהסיסמא החדשה היא: " + password;
            client.Send(ProjMail, email, subject, body);
        }
        public static void SendMailNotificationToStudent(ClassDTO newClass)
        {
            //Get only students
            List<tblStudent> studentList = db.tblStudent.Where(s => s.tblSuperStudent == null).ToList();
            //Brake function if there are no students in system.
            if (studentList.Count==0)
            {
                return;
            }
            //Create SmtpClient
            SmtpClient client = new SmtpClient(Host, Port)
            {
                Credentials = new NetworkCredential(ProjMail, Password),
                EnableSsl = true
            };
            //Mail subject
            string subject = "HelpMeStudent - התראות לשיעורים";
            //Mail body
            string body = "";
            //Take the List of tags from ClassDTO.
            List<string> allTagsInClass = newClass.Tags;
            foreach (tblStudent student in studentList)
            {
                string email = student.Email;
                string studentName = student.FullName;
                foreach (tblTags notificationTag in student.tblTags)
                {       
                    string tagName = notificationTag.TagName;
                    if (allTagsInClass.Contains(tagName))
                    {
                        //Create mail body.
                        body = CreateBodyMailForNotifications(newClass, studentName,email);
                        client.Send(ProjMail, email, subject, body);
                        //go to check next student.
                        break;
                    }
                }
            }
        }
        private static string CreateBodyMailForNotifications(ClassDTO newClass, string StudentName,string studentMail)
        {
            string studentName = StudentName;
            string classDate = newClass.ClassDate.ToString("MM/dd/yyyy");
            string className = newClass.ClassName;
            string superName = newClass.SuperName;
            string body = $"שלום רב,{studentName}\r\nאנו מעדכנים כי נוסף שיעור חדש המכיל תגיות אשר ביקשת לקבל עלייהם התראות.\r\nשם השיעור:{className}\r\nתאריך:{classDate}\r\nשם הסופר סטודנט:{superName}\r\n:תגיות השיעור\r\n";
            foreach (string tagName in newClass.Tags)
            {
                body += tagName + "\r\n";
            }
            body += "HelpMeStudent-צוות";
            return body;
        }
        public static void sendNewRequestToAdmin(RequestToJoinDTO request)
        {
            SmtpClient client = new SmtpClient(Host, Port)
            {
                Credentials = new NetworkCredential(ProjMail, Password),
                EnableSsl = true
            };
            string subject = "בקשת הצטרפות חדשה למערכת";
            string body = "שלום רב,\r\nהתקבלה בקשה חדשה להצטרפות למערכת\r\nשם סטודנט: " + request.FullName + "\r\nת.ז: " + request.StudentId + "\r\nטלפון: " + request.Phone;
            client.Send(ProjMail, ProjMail, subject, body);
        }
    }
}