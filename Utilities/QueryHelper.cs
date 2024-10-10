using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore;
using Task = PmsApi.Models.Task;
using PmsApi.Models;

namespace PmsApi.Utilities
{
    public static class QueryHelper
    {
        public static IQueryable<Task> ApplyIncludes(IQueryable<Task> query, string include){
            var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach(var item in includes){
                switch(item){
                    case "user":
                        query = query.Include(t => t.User);
                        break;
                    case "project":
                        query = query.Include(t => t.Project);
                        break;
                    case "attachments":
                        query = query.Include(t => t.Attachement);
                        break;
                }
            }
            return query;
        }

        public static IQueryable<Project> ApplyIncludesProject(IQueryable<Project> query, string include){
            var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach(var item in includes){
                switch(item){
                    case "tasks":
                        query = query.Include(p => p.Tasks);
                        break;
                    case "manager":
                        query = query.Include(p => p.Manager);
                        break;
                    case "category":
                        query = query.Include(p => p.Category);
                        break;
                }
            }
            return query;
        }

        public static IQueryable<User> ApplyIncludesUser(IQueryable<User> query, string include){
            var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach(var item in includes){
                switch(item){
                    case "projects":
                        query = query.Include(u => u.Projects);
                        break;
                    case "tasks":
                        query = query.Include(u => u.Tasks);
                        break;
                }
            }
            return query;
        }
    }
}