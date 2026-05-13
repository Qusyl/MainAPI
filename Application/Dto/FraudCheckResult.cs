using Domain.value;


namespace Application.Dto
{
    public class FraudCheckResult
    {
        public FraudDecision Decision { get; set; }

        public string RuleName { get; set; }

        public string Message { get; set; }

        public FraudCheckResult(FraudDecision decision, string ruleName, string message = "No reason for decline") {
            Decision = decision;
            RuleName = ruleName;
            Message = message;
        }
    }
}
