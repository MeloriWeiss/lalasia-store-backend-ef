// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
//
// namespace Lalasia_store.Models.Data;
//
// public class RefreshToken
// {
//     [Key]
//     public Guid Id { get; set; } = Guid.NewGuid();
//     public string Token { get; set; }
//     public DateTime ExpiresAt { get; set; }
//     
//     [ForeignKey(nameof(User))]
//     public Guid UserId { get; set; }
//     public virtual User User { get; set; }
// }