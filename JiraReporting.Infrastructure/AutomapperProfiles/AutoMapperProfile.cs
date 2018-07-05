using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using JiraReporting.JiraClient.Entities;
using JiraReporting.Models;

namespace JiraReporting.Infrastructure.AutomapperProfiles
{
    /// <summary>
    /// Automapper profile
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        /// </summary>
        public AutoMapperProfile()
        {
            CreateMap<AgileIssue, IssueDescriptionModel>()
                .ForMember(dest => dest.IssueLink,
                    opt => opt.ResolveUsing(src =>
                    {
                        // If issue type is subTask change issue Uri to parent(Task) Uri
                        if (src.Fields.IssueType.IsSubTask)
                        {
                            return "https://jira.tideworks.com/browse/" + src.Fields.Parent.Key;
                        }

                        return "https://jira.tideworks.com/browse/" + src.Key;
                    }))
                .ForMember(dest => dest.ParentTaskName,
                    otp => otp.ResolveUsing(src => src.Fields.IssueType.IsSubTask ? src.Fields.Parent.Fields.Summary : ""))
                .ForMember(dest => dest.Description,
                    opt => opt.ResolveUsing(src => src.Fields.Summary))
                .ForMember(dest => dest.IssueKey,
                    opt => opt.ResolveUsing(src =>
                    {
                        // If issue type is subTask change issue key to parent(Task) key
                        if (src.Fields.IssueType.IsSubTask)
                        {
                            return src.Fields.Parent.Key;
                        }

                        return src.Key;
                    }))
                .ForMember(src => src.IssueStatus, otp => otp.ResolveUsing(dest => dest.Fields.Status.Name));

            CreateMap<KeyValuePair<string, List<AgileIssue>>, ImpedimentItemModel>()
                .ForMember(dest => dest.Team,
                    opt => opt.ResolveUsing((src, dst, arg3, context) => context.Options.Items["teamName"].ToString().Replace(" Team", "")))
                .ForMember(dest => dest.Developer,
                    otp => otp.MapFrom(
                        src => src.Value.First().Fields.Assignee.DisplayName.Replace(" (Contractor)", "").Replace(" (contractor)", ""))).
                ForMember(dest => dest.ImpedimntIssueDescriptionModels,
                    otp => otp.MapFrom(src => src.Value));

            CreateMap<TeamMemberWorkIssuesModel, StatusItemModel>()
                .ForMember(dest => dest.CompletedIssueDescriptionModels,
                    otp => otp.MapFrom(src => src.GroupedWorkedIssues))
                .ForMember(dest => dest.FutureIssueDescriptionModels,
                    otp => otp.MapFrom(src => src.GroupedWillWorkIssues))
                .ForMember(dest => dest.Team,
                    opt => opt.ResolveUsing((src, dst, arg3, context) => context.Options.Items["teamName"].ToString().Replace(" Team", "")));

            CreateMap<TeamMemberTriageIssuesModel, TriageItemModel>()
                 .ForMember(dest => dest.Team,
                    opt => opt.ResolveUsing((src, dst, arg3, context) => context.Options.Items["teamName"].ToString().Replace(" Team", "")))
                 .ForMember(dest => dest.ImpedimentIssueDescriptionModels,
                    otp => otp.MapFrom(src => src.GropedImpedimentIssues))
                 .ForMember(dest => dest.WillWorkOnIssueDescriptionModels,
                    otp => otp.MapFrom(src => src.GroupedWillWorkIssues))
                 .ForMember(dest => dest.WorkedIssueDescriptionModels,
                    otp => otp.MapFrom(src => src.GroupedWorkedIssues));
        }
    }
}