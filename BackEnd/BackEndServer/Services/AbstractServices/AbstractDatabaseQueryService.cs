using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractDatabaseQueryService
    {
        List<Hashtable> executeSelectQuery(string sqlQuery, Hashtable parameters);
        
        void executeNonSelectQuery(string sqlQuery, Hashtable parameters);
    }
}