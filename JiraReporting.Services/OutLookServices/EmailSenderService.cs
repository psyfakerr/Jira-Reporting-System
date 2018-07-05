using System;
using System.Collections.Generic;
using System.Text;
using JiraReporting.Models;
using JiraReporting.Services.Interfaces;
using Exception = System.Exception;
using Outlook = Microsoft.Office.Interop.Outlook;


namespace JiraReporting.Services.OutLookServices
{
    public class EmailSenderService : IEmailSenderService
    {
        private Outlook.Application _outlookAplication;
        
        private string _subject = "Akvelon status " + DateTime.Now.ToShortDateString();

        //const data for letter
        private const string _htmlBody = "<html><body>";
        private const string _hiText = "<div>Hi all, </div>";
        private const string _text = "<p>Here is the combined status of Akvelon Mainsail team.<p>";
        private const string _textImpediment = "<div><b>Impediments</b></div>";
        private const string _textStastus = "<div><b>Status</b></div>";

        //const string for create table
        private const string _tableStyle = "<table style=\"width:100%;border-collapse:collapse;vertical-align:top\"><tr>";
        private const string _tdStyleForTable = "<td style=\"border:1px solid black;padding-left:5px;vertical-align:top\">";

        //constuctor
        public EmailSenderService()
        {
            _outlookAplication = new Outlook.Application();
        }

        public void FormationTablesForLetter(List<ImpedimentItemModel> listForImpedimentTable,
            List<StatusItemModel> listForStatusItemModel, List<RecipientModel> recipients, List<TriageItemModel> triageList)
        {
            //creating tables impediment and status
            StringBuilder impedimentTable = CreateImpedimentTable(listForImpedimentTable, triageList);
            StringBuilder stastusTable = CreateStatusTable(listForStatusItemModel, triageList);

            //sent the letter and check the result
            bool resultSend = FormationSendLetter(impedimentTable, stastusTable, recipients);
            if (resultSend == true)
            {
                Console.WriteLine("Your message is sent");
            }
        }

        //method create impediment table
        public StringBuilder CreateImpedimentTable(List<ImpedimentItemModel> impedimentList, List<TriageItemModel> triageList)
        {
            string resultString = "";
            string str = "";
            string triageString = "";

            StringBuilder impedimentTable = new StringBuilder();
            impedimentTable.Append(_tableStyle);

            List<string> nameColumn = new List<string>
            {
                "Team",
                "Developer",
                "Description"
            };

            foreach (string column in nameColumn)
            {
                impedimentTable.Append(_tdStyleForTable + column + "</td>");
            }
            impedimentTable.Append("</tr>");
            foreach (ImpedimentItemModel row in impedimentList)
            {
                impedimentTable.Append("<tr>");

                impedimentTable.Append(_tdStyleForTable + row.Team + "</td>");
                impedimentTable.Append(_tdStyleForTable + row.Developer + "</td>");
                
                //check description on null
                if (row.ImpedimntIssueDescriptionModels.Count == 0)
                {
                    resultString = "";
                    resultString += _tdStyleForTable;
                    resultString += "<div><a>  </a></div>";
                    resultString += "</td>";
                }
                if (row.ImpedimntIssueDescriptionModels.Count > 0)
                {
                    foreach (IssueDescriptionModel data in row.ImpedimntIssueDescriptionModels)
                    {

                        //for ToLower case IssueStatus
                        string issueStatus = ToLowerCaseIssueStatus(data);

                        if (row.ImpedimntIssueDescriptionModels.Count > 1)
                        {
                            if (str == "")
                            {
                                str += _tdStyleForTable;
                                str += PushDataToStringForImpedimentTable(data.IssueKey, data.IssueLink, data.Description, issueStatus);
                            }
                            else
                            {
                                str += PushDataToStringForImpedimentTable(data.IssueKey, data.IssueLink, data.Description, issueStatus);
                            }
                        }
                        if (row.ImpedimntIssueDescriptionModels.Count == 1)
                        {
                            str = "";
                            str += _tdStyleForTable;
                            str += PushDataToStringForImpedimentTable(data.IssueKey, data.IssueLink, data.Description, issueStatus);
                        }
                    }
                    resultString += str;
                }

                foreach (TriageItemModel triageRow in triageList)
                {
                    if (triageRow.ImpedimentIssueDescriptionModels.Count > 0)
                    {
                        if (row.Team == triageRow.Team && row.Developer == triageRow.Developer)
                        {
                            foreach (IssueDescriptionModel triageData in triageRow.ImpedimentIssueDescriptionModels)
                            {
                                //for ToLower case IssueStatus
                                string issueStatus = ToLowerCaseIssueStatus(triageData);

                                if (triageRow.ImpedimentIssueDescriptionModels.Count > 1)
                                {
                                    if (triageString == "")
                                    {
                                        triageString += _tdStyleForTable;
                                        triageString += PushDataToStringForImpedimentTable(triageData.IssueKey, triageData.IssueLink, triageData.Description, issueStatus);
                                    }
                                    else
                                    {
                                        triageString += PushDataToStringForImpedimentTable(triageData.IssueKey, triageData.IssueLink, triageData.Description, issueStatus);
                                    }
                                }
                                if (triageRow.ImpedimentIssueDescriptionModels.Count == 1)
                                {
                                    triageString = "";
                                    triageString += _tdStyleForTable;
                                    triageString += PushDataToStringForImpedimentTable(triageData.IssueKey, triageData.IssueLink, triageData.Description, issueStatus);
                                }
                            }
                            triageString += "<b> </b>";
                            resultString += triageString;
                        }
                    }
                    resultString += "</td>";
                    impedimentTable.Append(resultString);
                    impedimentTable.Append("</tr>");


                    resultString = "";
                    str = "";
                    triageString = "";
                }
            }
            impedimentTable.Append("</table>");
            return impedimentTable;
        }

        //method create status table
        public StringBuilder CreateStatusTable(List<StatusItemModel> statusList, List<TriageItemModel> triageList)
        {
            StringBuilder statusTable = new StringBuilder();
            statusTable.Append(_tableStyle);

            List<string> nameColumn = new List<string>
            {
                "Team",
                "Developer",
                "Worked on",
                "Will work on"
            };
            string resultString = "";
            string str = "";
            string triageString = "";
            foreach (string column in nameColumn)
            {
                statusTable.Append(_tdStyleForTable + column + "</td>");
            }
            statusTable.Append("</tr>");
            foreach (StatusItemModel row in statusList)
            {
                statusTable.Append("<tr>");

                statusTable.Append(_tdStyleForTable + row.Team + "</td>");
                statusTable.Append(_tdStyleForTable + row.Developer + "</td>");

                if (row.CompletedIssueDescriptionModels.Count > 0)
                {
                    foreach (IssueDescriptionModel data in row.CompletedIssueDescriptionModels)
                    {
                        if (row.CompletedIssueDescriptionModels.Count > 1)
                        {
                            if (resultString == "" && str == "")
                            {
                                str += _tdStyleForTable;
                                if (data.ParentTaskName == "")
                                {
                                    str += ParentNameNULL(data.IssueKey, data.IssueLink, data.Description);
                                }
                                else
                                {
                                    str += ParentNameNotNULL(data.IssueKey, data.IssueLink, data.ParentTaskName, data.Description);
                                }
                            }
                            else
                            {
                                if (data.ParentTaskName == "")
                                {
                                    str += ParentNameNULL(data.IssueKey, data.IssueLink, data.Description);
                                }
                                else
                                {
                                    str += ParentNameNotNULL(data.IssueKey, data.IssueLink, data.ParentTaskName, data.Description);
                                }
                            }
                        }
                        if (row.CompletedIssueDescriptionModels.Count == 1)
                        {
                            str = "";
                            str += _tdStyleForTable;
                            if (data.ParentTaskName == "")
                            {
                                str += ParentNameNULL(data.IssueKey, data.IssueLink, data.Description);
                            }
                            else
                            {
                                str += ParentNameNotNULL(data.IssueKey, data.IssueLink, data.ParentTaskName, data.Description);
                            }
                        }
                    }
                    resultString += str;
                }
                //create "Worked on" column
                
                foreach (TriageItemModel triageRow in triageList)
                {
                    if (triageRow.WorkedIssueDescriptionModels.Count == 0 && 
                        row.CompletedIssueDescriptionModels.Count == 0 && resultString == "")
                    {
                        resultString = "";
                        resultString += _tdStyleForTable;
                        resultString += "<div><a>  </a></div>";
                    }


                    if (triageRow.WorkedIssueDescriptionModels.Count > 0)
                    {
                        if (row.Team == triageRow.Team && row.Developer == triageRow.Developer)
                        {
                            foreach (IssueDescriptionModel triageData in triageRow.WorkedIssueDescriptionModels)
                            {
                                if (triageRow.WorkedIssueDescriptionModels.Count > 1)
                                {
                                    if (resultString == "" && triageString == "")
                                    {
                                        triageString += _tdStyleForTable;
                                        if (triageData.ParentTaskName == "")
                                        {
                                            triageString += ParentNameNULL(triageData.IssueKey, triageData.IssueLink, triageData.Description);
                                        }
                                        else
                                        {
                                            triageString += ParentNameNotNULL(triageData.IssueKey, triageData.IssueLink, triageData.ParentTaskName, triageData.Description);
                                        }
                                    }
                                    else
                                    {
                                        if (triageData.ParentTaskName == "")
                                        {
                                            triageString += ParentNameNULL(triageData.IssueKey, triageData.IssueLink, triageData.Description);
                                        }
                                        else
                                        {
                                            triageString += ParentNameNotNULL(triageData.IssueKey, triageData.IssueLink, triageData.ParentTaskName, triageData.Description);
                                        }
                                    }
                                }
                                if (triageRow.WorkedIssueDescriptionModels.Count == 1)
                                {
                                    triageString = "";
                                    if(str == "")
                                    {
                                        triageString += _tdStyleForTable;
                                    }

                                    if (triageData.ParentTaskName == "")
                                    {
                                        triageString += ParentNameNULL(triageData.IssueKey, triageData.IssueLink, triageData.Description);
                                    }
                                    else
                                    {
                                        triageString += ParentNameNotNULL(triageData.IssueKey, triageData.IssueLink, triageData.ParentTaskName, triageData.Description);
                                    }
                                }
                            }
                            resultString += triageString;
                        }
                    }
                }
                resultString += "</td>";
                // end creating "Worked on" column and push to table
                statusTable.Append(resultString);

                resultString = "";
                str = "";
                triageString = "";


                //create "Will work on" column
                if (row.FutureIssueDescriptionModels.Count > 0)
                {
                    foreach (IssueDescriptionModel data in row.FutureIssueDescriptionModels)
                    {
                        if (row.FutureIssueDescriptionModels.Count > 1)
                        {
                            if (resultString == "" && str == "")
                            {
                                str += _tdStyleForTable;
                                if (data.ParentTaskName == "")
                                {
                                    str += ParentNameNULL(data.IssueKey, data.IssueLink, data.Description);
                                }
                                else
                                {
                                    str += ParentNameNotNULL(data.IssueKey, data.IssueLink, data.ParentTaskName, data.Description);
                                }
                            }
                            else
                            {
                                if (data.ParentTaskName == "")
                                {
                                    str += ParentNameNULL(data.IssueKey, data.IssueLink, data.Description);
                                }
                                else
                                {
                                    str += ParentNameNotNULL(data.IssueKey, data.IssueLink, data.ParentTaskName, data.Description);
                                }
                            }
                        }
                        if (row.FutureIssueDescriptionModels.Count == 1)
                        {
                            str = "";
                            str += _tdStyleForTable;
                            if (data.ParentTaskName == "")
                            {
                                str += ParentNameNULL(data.IssueKey, data.IssueLink, data.Description);
                            }
                            else
                            {
                                str += ParentNameNotNULL(data.IssueKey, data.IssueLink, data.ParentTaskName, data.Description);
                            }
                        }
                    }
                    resultString += str;
                }


                foreach (TriageItemModel triageRow in triageList)
                {
                    if (triageRow.WillWorkOnIssueDescriptionModels.Count == 0 && 
                        row.FutureIssueDescriptionModels.Count == 0 && resultString == "")
                    {
                        resultString = "";
                        resultString += _tdStyleForTable;
                        resultString += "<div><a>  </a></div>";
                    }
                    // push triage data to column will work on
                    if (triageRow.WillWorkOnIssueDescriptionModels.Count > 0)
                    {
                        if (row.Team == triageRow.Team && row.Developer == triageRow.Developer)
                        {
                            foreach (IssueDescriptionModel triageData in triageRow.WillWorkOnIssueDescriptionModels)
                            {
                                if (triageRow.WillWorkOnIssueDescriptionModels.Count > 1)
                                {
                                    if (resultString == "" && triageString == "")
                                    {
                                        triageString += _tdStyleForTable;
                                        if (triageData.ParentTaskName == "")
                                        {
                                            triageString += ParentNameNULL(triageData.IssueKey, triageData.IssueLink, triageData.Description);
                                        }
                                        else
                                        {
                                            triageString += ParentNameNotNULL(triageData.IssueKey, triageData.IssueLink, triageData.ParentTaskName, triageData.Description);
                                        }
                                    }
                                    else
                                    {
                                        if (triageData.ParentTaskName == "")
                                        {
                                            triageString += ParentNameNULL(triageData.IssueKey, triageData.IssueLink, triageData.Description);
                                        }
                                        else
                                        {
                                            triageString += ParentNameNotNULL(triageData.IssueKey, triageData.IssueLink, triageData.ParentTaskName, triageData.Description);
                                        }
                                    }
                                }
                                if (triageRow.WillWorkOnIssueDescriptionModels.Count == 1)
                                {
                                    triageString = "";
                                    if (str == "")
                                    {
                                        triageString += _tdStyleForTable;
                                    }
                                    if (triageData.ParentTaskName == "")
                                    {
                                        triageString += ParentNameNULL(triageData.IssueKey, triageData.IssueLink, triageData.Description);
                                    }
                                    else
                                    {
                                        triageString += ParentNameNotNULL(triageData.IssueKey, triageData.IssueLink, triageData.ParentTaskName, triageData.Description);
                                    }
                                }
                            }
                            resultString += triageString;
                        }
                    }
                }
                
                resultString += "</td>";
                // end creating "Will work on" column and push to table
                statusTable.Append(resultString);
                statusTable.Append("</tr>");


                resultString = "";
                str = "";
                triageString = "";

                
            }
            statusTable.Append("</table>");
            return statusTable;
        }

        public bool FormationSendLetter(StringBuilder impedimentTable, StringBuilder stastusTable, List<RecipientModel> recipientMail)
        {
            try
            {
                Outlook.NameSpace nameSpace = _outlookAplication.GetNamespace("MAPI");
                Outlook.MAPIFolder mapiFolder = nameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);

                Outlook.MailItem message = (Outlook.MailItem)_outlookAplication.CreateItem(Outlook.OlItemType.olMailItem);

                //formation letter
                message.Subject = _subject;
                message.HTMLBody = _htmlBody;
                message.HTMLBody += _hiText;
                message.HTMLBody += _text;
                message.HTMLBody += _textImpediment;
                message.HTMLBody += impedimentTable;
                message.HTMLBody += "<br></br>";
                message.HTMLBody += _textStastus;
                message.HTMLBody += stastusTable;

                foreach (var recipMail in recipientMail)
                {
                    Outlook.Recipients newRecipients = (Outlook.Recipients)message.Recipients;
                    Outlook.Recipient recipient;
                    
                    recipient = (Outlook.Recipient)newRecipients.Add(recipMail.MailAddress);
                    recipient.Resolve();
                }
                message.Send();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public string ParentNameNULL(string issueKey, string issueLink, string description)
        {
            string str = "";
            if (issueKey == "TRIAGE-14651" || issueKey == "TRIAGE-15376")
            {
                str = "<div style=\"color:red\">" + issueKey + " " + "<a href='" + issueLink + "'>" + description + " " + "</a></div>";
            }
            else
            {
                str = "<div>" + issueKey + " " + "<a href='" + issueLink + "'>" + description + " " + "</a></div>";
            }
            
            return str;
        }

        public string ParentNameNotNULL(string issueKey, string issueLink, string parentTaskName, string description)
        {
            string str = "";
            if (issueKey == "TRIAGE-14651" || issueKey == "TRIAGE-15376")
            {
                str = "<div style =\"color:red\">" + issueKey + " " + "<a href='" + issueLink + "'>" + parentTaskName + " " + "</a>" + description + "</div>";
            }
            else
            {
                str = "<div>" + issueKey + " " + "<a href='" + issueLink + "'>" + parentTaskName + " " + "</a>" + description + "</div>";
            }

            return str;
        }

        public string PushDataToStringForImpedimentTable(string issueKey, string issueLink, string description, string issueStatus)
        {
            string str = "";

            if(issueKey == "TRIAGE-14651" || issueKey == "TRIAGE-15376")
            {
                str = "<div style =\"color:red\">" + issueKey + " " + "<a href='" + issueLink + "'>" + description + "</a>" + " " + issueStatus + "</div>";
            }
            else
            {
                str = "<div>" + issueKey + " " + "<a href='" + issueLink + "'>" + description + "</a>" + " " + issueStatus + "</div>";
            }
            
            return str;
        }

        public string ToLowerCaseIssueStatus(IssueDescriptionModel data)
        {
            string issueStatus = "";
            issueStatus += data.IssueStatus[0];
            string issueStatusStr = data.IssueStatus.ToLower();
            for (int i = 0; i < issueStatusStr.Length; i++)
            {
                if (i > 0)
                {
                    issueStatus += issueStatusStr[i];
                }
            }
            return issueStatus;
        }
    }
}