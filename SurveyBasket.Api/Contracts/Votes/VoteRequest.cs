namespace SurveyBasket.Api.Contracts.Votes;

public record VoteRequest(
    List<VoteAnswerRequest> Answers
);
