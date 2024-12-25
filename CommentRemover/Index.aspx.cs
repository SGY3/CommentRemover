using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;

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
                ShowToaster("error", ex.Message);
            }
        }

        protected void btnRemoveComment_Click(object sender, EventArgs e)
        {
            try
            {
                string selection = radType.SelectedValue;
                if (selection == "Text")
                {
                    string outputData = HandleText(txtText.Text);
                    string filePath = CreateSingleFile(outputData);
                    DownloadFile(Server.MapPath(filePath));
                }
                else if (selection == "File")
                {
                    List<string> files = HandleFile();
                    int fileCount = files.Count;
                    if (fileCount == 0)
                        return;
                    if (files.Count == 1)
                        DownloadFile(files[0]);
                    else
                    {
                        string zipLocation = "~/TempZip/";
                        string zipName = "CommentRemoved_" + DateTime.Now.ToString("yyyyMMddhhmmss") + "/";
                        string zipPath = Path.Combine(zipLocation, zipName);
                        if (!Directory.Exists(zipPath))
                            Directory.CreateDirectory(Server.MapPath(zipPath));
                        foreach (string file in files)
                        {
                            File.Copy(Server.MapPath(file), Server.MapPath(Path.Combine(zipPath, Path.GetFileName(file))));
                        }
                        string fileName = "CommentRemoved_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".zip";
                        string finalZipPath = Path.Combine(zipLocation, fileName);
                        ZipFile.CreateFromDirectory(Server.MapPath(zipPath), Server.MapPath(finalZipPath));
                        DownloadFile(Server.MapPath(finalZipPath));
                    }

                }
            }
            catch (Exception ex)
            {
                ShowToaster("error", ex.Message);
            }
        }
        private string HandleText(string textData)
        {
            try
            {
                string[] lines = textData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                StringBuilder stringBuilder = new StringBuilder();

                int cnt = 0;
                if (radCommentType.SelectedValue == "SQL")
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();
                        if (line.IndexOf("begin", StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            break;
                        }
                        cnt++;
                    }
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
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                ShowToaster("error", ex.Message);
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
        private List<string> HandleFile()
        {
            List<string> fileResult = new List<string>();
            try
            {
                string folderPath = "~/Temp/";
                string serverPath = Server.MapPath(folderPath);

                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);

                foreach (HttpPostedFile httpPostedFile in fuFile.PostedFiles)
                {
                    string fileName = httpPostedFile.FileName + "~~" + Guid.NewGuid().ToString() + ".txt";
                    string finalFile = Path.Combine(serverPath, fileName);
                    httpPostedFile.SaveAs(finalFile);
                    fileResult.Add(CreateSingleFile(HandleText(File.ReadAllText(finalFile))));
                }
            }
            catch (Exception ex)
            {
                ShowToaster("error", ex.Message);
            }
            return fileResult;
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
                    ShowToaster("error", ex.Message);
                    return "";
                }

            }
            catch (Exception ex)
            {
                ShowToaster("error", ex.Message);
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
                ShowToaster("error", ex.Message);
            }
        }
        private void ShowToaster(string type, string message)
        {
            string script = $"ShowToast('success', '{message}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastrNotification", script, true);
        }
    }
}