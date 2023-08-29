﻿using Box.V2.Models;

namespace Apps.Box.Dtos;

public abstract class BoxItemDto
{
    protected BoxItemDto(BoxItem item)
    {
        Path = "/" + string.Join('/', item.PathCollection.Entries.Skip(1).Select(p => p.Name));
        Name = item.Name;
        Size = item.Size;
        Description = item.Description;
    }
    
    public string Path { get; set; }
    public string Name { get; set; }
    public long? Size { get; set; }
    public string Description { get; set; }
}