using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CommentRemover
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void radType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selection = radType.SelectedValue;
                divFile.Visible = false;
                divText.Visible = false;
                if (selection == "Text")
                {
                    divText.Visible = true;
                }
                else
                {
                    divFile.Visible = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void btnRemoveComment_Click(object sender, EventArgs e)
        {
            try
            {
                string selection = radType.SelectedValue;
                if (selection == "Text")
                {
                    string outputData = HandleText();
                    string filePath = CreateSingleFile(outputData);
                    DownloadFile(Server.MapPath(filePath));
                }
            }
            catch (Exception ex)
            {

            }
        }
        private string HandleText(string formatType = "SQL")
        {
            try
            {
                string[] lines = txtText.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                StringBuilder stringBuilder = new StringBuilder();
                if (formatType == "SQL")
                {
                    int cnt = 0;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();
                        if (line.IndexOf("begin", StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            break;
                        }
                        cnt++;
                    }

                    for (int i = cnt; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();

                        if (line.Contains("/*")) //Multiline Comment
                        {
                            string newLine = RemoveMultilineComment(line);
                            if (newLine.Contains("--"))
                            {
                                int startPostion = newLine.IndexOf("--");
                                stringBuilder.Append(newLine.Remove(startPostion) + " ");
                            }
                            else
                            {
                                stringBuilder.Append(newLine + " ");
                            }
                        }
                        else if (line.Contains("--"))
                        {
                            int startPostion = line.IndexOf("--");
                            stringBuilder.Append(line.Remove(startPostion) + " ");
                        }
                        else
                        {
                            stringBuilder.Append(line + " ");
                        }
                    }
                }
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private string RemoveMultilineComment(string line)
        {
            if (line.Contains("/*")) //Multiline Comment
            {
                int startPosotion = line.IndexOf("/*");
                int lastPosition = line.IndexOf("*/") + 2;

                string newLine = line.Remove(startPosotion, (lastPosition - startPosotion));
                return RemoveMultilineComment(newLine);
            }
            else
            {
                return line;
            }
        }
        private void HandleFile()
        {
            try
            {
                string selection = radType.SelectedValue;
            }
            catch (Exception ex)
            {

            }
        }
        private string CreateSingleFile(string fileContent)
        {
            try
            {
                string userFileName = txtFileName.Text.Trim();
                string fileName = (userFileName != "" ? userFileName : Guid.NewGuid().ToString()) + ".txt";

                string folderPath = "~/Temp/";
                string serverPath = Server.MapPath(folderPath);

                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);

                string finalFile = Path.Combine(serverPath, fileName);
                try
                {
                    // Using 'StreamWriter' to create and write to the file
                    using (StreamWriter writer = new StreamWriter(finalFile))
                    {
                        writer.WriteLine(fileContent);  // Write the content to the file
                    }
                    return Path.Combine(folderPath, fileName);
                }
                catch (Exception ex)
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                return "";
            }
        }
        private void DownloadFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var fi = new FileInfo(filePath);
                    Response.Clear();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fi.Name);
                    Response.WriteFile(filePath);
                    Response.End();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}