using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFcore.Domain;

public class Coach : BaseDomainModel
{
    //anything called ID automatically set to PK
    public string Name { get; set; }
    public virtual Team? Team { get; set; }

}