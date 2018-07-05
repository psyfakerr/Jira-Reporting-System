using System.Collections.Generic;
using System.Text;
using JiraReporting.Models;

namespace JiraReporting.Services.Interfaces
{
    public interface IEmailSenderService
    {
        void FormationTablesForLetter(List<ImpedimentItemModel> listForImpedimentTable,
            List<StatusItemModel> listForStatusItemModel, List<RecipientModel> recipient, List<TriageItemModel> triageList);

        StringBuilder CreateImpedimentTable(List<ImpedimentItemModel> impedimentList, List<TriageItemModel> triageList);
        //method for impediment table
        string PushDataToStringForImpedimentTable(string issueKey, string issueLink, string description, string issueStatus);  


        StringBuilder CreateStatusTable(List<StatusItemModel> statusList, List<TriageItemModel> triageList);
        //methods for status table
        string ParentNameNULL(string issueKey, string issueLink, string description);
        string ParentNameNotNULL(string issueKey, string issueLink, string parentTaskName, string description);
        string ToLowerCaseIssueStatus(IssueDescriptionModel data);

        bool FormationSendLetter(StringBuilder impedimentTable, StringBuilder stastusTable, List<RecipientModel> recipientMail);

    }
}
