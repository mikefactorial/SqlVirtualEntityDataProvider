using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MikeFactorial.Xrm.Plugins.DataProviders
{
    public abstract class BaseExecutionContext
    {
        public IOrganizationService Service { get; protected set; }

        public ITracingService TracingService { get; protected set; }

        public IServiceProvider ServiceProvider { get; protected set; }

        public Entity TargetEntity
        {
            get
            {
                if (Context != null)
                {
                    if (Context.InputParameters != null && 
                    Context.InputParameters.Contains("Target") &&
                    Context.InputParameters["Target"] is Entity)
                    {
                        return (Entity)Context.InputParameters["Target"];
                    }
                    else if (Context.PreEntityImages != null &&
                    Context.PreEntityImages.Count > 0)
                    {
                        return Context.PreEntityImages.Values.FirstOrDefault();
                    }
                    else if (Context.PostEntityImages != null &&
                    Context.PostEntityImages.Count > 0)
                    {
                        return Context.PostEntityImages.Values.FirstOrDefault();
                    }
                }

                return null;
            }

            set
            {
                Context.InputParameters["Target"] = value;
            }
        }


        protected IExecutionContext Context { get; set; }

        public void Trace(string format, params object[] args)
        {
            TracingService.Trace(format, args);
        }

        internal void XrmTrace(string format, params object[] args)
        {
            Trace("[XrmTrace] " + format, args);
        }

        protected void Init()
        {
            LogTheContext(Context);
        }

        private void LogTheContext(IExecutionContext context)
        {
            if (context == null)
            {
                return;
            }

            var contextDetails = new List<string>
            {
                "Context details: " +
                "Step: " + GetStep(context),
                "Msg: " + context.MessageName,
                "Mode: " + context.Mode,
                "Depth: " + context.Depth,
                "Corr: " + context.CorrelationId,
                "Type: " + context.PrimaryEntityName,
                "Id: " + context.PrimaryEntityId,
                "User: " + context.UserId,
                "IUser: " + context.InitiatingUserId
            };

            //Insert 'Stage' after 'Step' if it is a Plugin Context
            if (context is IPluginExecutionContext pluginContext)
            {
                contextDetails.Insert(2, "Stage: " + pluginContext.Stage);
            }

            XrmTrace(string.Join(Environment.NewLine, contextDetails));
        }

        private string GetStep(IExecutionContext context)
        {
            if (context.OwningExtension == null)
            {
                return "not defined";
            }
            else if (!string.IsNullOrEmpty(context.OwningExtension.Name))
            {
                return context.OwningExtension.Name;
            }
            else
            {
                return context.OwningExtension.Id.ToString();
            }
        }
    }
}
