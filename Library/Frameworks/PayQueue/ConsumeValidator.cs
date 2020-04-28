using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class ConsumeValidator<T>where T: IServiceDefinition, new()
    {
        
        public ConsumeValidator(Func<IServiceImpl<T>> impl) 
        {
            
        }

        public void Validate() 
        {
            
        }

    }

}