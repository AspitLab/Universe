using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

/*
 * Needs to set less secure apps on when sending files
 * https://www.google.com/settings/u/1/security/lesssecureapps?pageId=none
 * */

public class Mailtest : MonoBehaviour {

    private 


    void SendMail() {
        Application.OpenURL("mailto:?subject=subject&body=bodytesting&Attachment=" + Application.persistentDataPath + "\\screen.png");
        Debug.Log("SendEmail");
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Application.CaptureScreenshot(Application.persistentDataPath + "\\screen.png");
            Debug.Log("Screenshot Taken");
            Invoke("MailSendWithAttachment", 2);
        }
	}

    private void MailSendWithAttachment() {
        MailMessage mail = new MailMessage();    // create email info

        mail.From = new MailAddress("audiologmusic@gmail.com");
        mail.To.Add("abrp@khoravr.com");
        mail.Subject = "Test Email";
        mail.Body = "This is for testing";

        // This only works if I write out the full path instead of calling 'uploadFile'
        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(Application.persistentDataPath + "\\screen.png");
        mail.Attachments.Add(attachment);

        // smpt server
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("audiologmusic@gmail.com", "Runner242") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        };
        smtpServer.Send(mail);
    }
}



