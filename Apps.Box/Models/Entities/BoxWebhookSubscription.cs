namespace Apps.Box.Models.Entities;

public record BoxWebhookSubscription(string FolderId, string CallbackUrl, IReadOnlyCollection<string> Triggers);
