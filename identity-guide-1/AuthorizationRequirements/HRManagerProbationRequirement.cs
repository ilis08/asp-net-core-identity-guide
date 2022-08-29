using Microsoft.AspNetCore.Authorization;

namespace identity_guide_1.AuthorizationRequirements
{
    public class HRManagerProbationRequirement : IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationMonth)
        {
            ProbationMonth = probationMonth;
        }

        public int ProbationMonth { get; }
    }

    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "EmploymentDate"))
            {
                return Task.CompletedTask;
            }

            var empDate = DateTime.Parse(context.User.FindFirst(x => x.Type == "EmploymentDate").Value);

            var period = DateTime.Now - empDate;

            if (period.Days > 30 * requirement.ProbationMonth)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
