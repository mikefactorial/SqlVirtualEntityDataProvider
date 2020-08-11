using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MikeFactorial.Xrm.Plugins.DataProviders
{
    /// <summary>
    /// The stage of a plugin.
    /// </summary>
    public enum PluginStages
    {
        /// <summary>
        /// Occurs prior to validation of the entity.
        /// </summary>
        PreValidation = 10,

        /// <summary>
        /// Occurs prior to the operation in the system.
        /// </summary>
        PreOperation = 20,

        /// <summary>
        /// Occurs during the main operation.
        /// </summary>
        MainOperation = 30,

        /// <summary>
        /// Occurs after the main operation.
        /// </summary>
        PostOperation = 40
    }

    /// <summary>
    /// Plugin Mode either Sync or Async
    /// </summary>
    public enum PluginMode
    {
        /// <summary>
        /// Synchronous Plugin
        /// </summary>
        Sync = 0,

        /// <summary>
        /// Asynchronous Plugin
        /// </summary>
        Async = 1            
    }

    public abstract class PluginBase : IPlugin
    {
        protected PluginBase()
        {
        }

        public virtual void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                PluginExecutionContext context = new PluginExecutionContext(serviceProvider);

                switch (context.PluginContext.MessageName)
                {
                    case "Create":
                        ExecuteCreate(context);

                        break;
                    case "Delete":
                        ExecuteDelete(context);

                        break;
                    case "Retrieve":
                        ExecuteRetrieve(context);

                        break;
                    case "RetrieveMultiple":
                        ExecuteRetrieveMultiple(context);

                        break;
                    case "Update":
                        ExecuteUpdate(context); 

                        break;
                    case "Associate":
                        ExecuteAssociate(context);

                        break;
                    case "Disassociate":
                        ExecuteDisassociate(context);
                        break;
                    default:
                        throw new NotImplementedException($"The message: {context.PluginContext.MessageName} is not supported");
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message, e);
            }
        }

        /// <summary>
        /// Handles the entity pre create message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPreCreateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post create message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostCreateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post create asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostCreateAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre delete message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPreDeleteMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post delete message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostDeleteMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post delete asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostDeleteAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre retrieve message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPreRetrieveMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post retrieve message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostRetrieveMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post retrieve asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostRetrieveAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity retrieve message.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void HandleEntityRetrieveMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre retrieve multiple message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPreRetrieveMultipleMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post retrieve multiple message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostRetrieveMultipleMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post retrieve multiple asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostRetrieveMultipleAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity retrieve multiple message.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void HandleEntityRetrieveMultipleMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre update message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPreUpdateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post update message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostUpdateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post update asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostUpdateAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre associate message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPreAssociateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post associate message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostAssociateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post associate asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostAssociateAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre disassociate message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPreDisassociateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post disassociate message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostDisassociateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post disassociate asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandleEntityPostDisassociateAsyncMessage(PluginExecutionContext context)
        {
        }

        private void ExecuteCreate(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandleEntityPreCreateMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandleEntityPostCreateAsyncMessage(context);
                }
                else
                {
                    HandleEntityPostCreateMessage(context);
                }
            }
            else
            {
                ThrowMessageNotFoundError(context.PluginContext.MessageName, context.PluginContext.Stage);
            }
        }

        private void ExecuteDelete(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandleEntityPreDeleteMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandleEntityPostDeleteAsyncMessage(context);
                }
                else
                {
                    HandleEntityPostDeleteMessage(context);
                }
            }
            else
            {
                ThrowMessageNotFoundError(context.PluginContext.MessageName, context.PluginContext.Stage);
            }
        }

        private void ExecuteRetrieve(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandleEntityPreRetrieveMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandleEntityPostRetrieveAsyncMessage(context);
                }
                else
                {
                    HandleEntityPostRetrieveMessage(context);
                }
            }
            else
            {
                HandleEntityRetrieveMessage(context);
            }
        }

        private void ExecuteRetrieveMultiple(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandleEntityPreRetrieveMultipleMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandleEntityPostRetrieveMultipleAsyncMessage(context);
                }
                else
                {
                    HandleEntityPostRetrieveMultipleMessage(context);
                }
            }
            else
            {
                HandleEntityRetrieveMultipleMessage(context);
            }
        }

        private void ExecuteUpdate(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandleEntityPreUpdateMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandleEntityPostUpdateAsyncMessage(context);
                }
                else
                {
                    HandleEntityPostUpdateMessage(context);
                }
            }
            else
            {
                ThrowMessageNotFoundError(context.PluginContext.MessageName, context.PluginContext.Stage);
            }
        }

        private void ExecuteAssociate(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandleEntityPreAssociateMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandleEntityPostAssociateAsyncMessage(context);
                }
                else
                {
                    HandleEntityPostAssociateMessage(context);
                }
            }
            else
            {
                ThrowMessageNotFoundError(context.PluginContext.MessageName, context.PluginContext.Stage);
            }
        }

        private void ExecuteDisassociate(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandleEntityPreDisassociateMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandleEntityPostDisassociateAsyncMessage(context);
                }
                else
                {
                    HandleEntityPostDisassociateMessage(context);
                }
            }
            else
            {
                ThrowMessageNotFoundError(context.PluginContext.MessageName, context.PluginContext.Stage);
            }
        }

        private bool IsPreOperation(PluginExecutionContext context)
        {
            return context.PluginContext.Stage == (int)PluginStages.PreValidation || context.PluginContext.Stage == (int)PluginStages.PreOperation;
        }

        private bool IsPostOperation(PluginExecutionContext context)
        {
            return context.PluginContext.Stage == (int)PluginStages.PostOperation;
        }

        private bool IsAsyncPlugin(PluginExecutionContext context)
        {
            return context.PluginContext.Mode == (int)PluginMode.Async;
        }

        private void ThrowMessageNotFoundError(string messageName, int stage)
        {
            throw new InvalidPluginExecutionException($"The message '{messageName}' is not supported in stage {stage}");
        }
    }
}
