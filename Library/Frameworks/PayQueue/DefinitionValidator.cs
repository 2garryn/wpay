using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class DefinitionValidator<T> where T: IServiceDefinition, new()
    {
        
        public void Validate()
        {
            var def =  new T();

        }
    }


}