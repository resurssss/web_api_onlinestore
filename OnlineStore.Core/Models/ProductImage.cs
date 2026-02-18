using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Core.Models;

public class ProductImage : BaseEntity
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    public int FileId { get; set; }
    
    public bool IsMain { get; set; } = false;
    
    public int Order { get; set; } = 0;
    
    // Навигационные свойства
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;
    
    [ForeignKey("FileId")]
    public virtual FileMetadata File { get; set; } = null!;
}