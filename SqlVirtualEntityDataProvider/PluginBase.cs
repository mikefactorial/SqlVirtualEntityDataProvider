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
        public virtual void HandlePreCreateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post create message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostCreateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post create asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostCreateAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre delete message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePreDeleteMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post delete message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostDeleteMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post delete asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostDeleteAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre retrieve message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePreRetrieveMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post retrieve message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostRetrieveMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post retrieve asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostRetrieveAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity retrieve message.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void HandleRetrieveMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre retrieve multiple message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePreRetrieveMultipleMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post retrieve multiple message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostRetrieveMultipleMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post retrieve multiple asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostRetrieveMultipleAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity retrieve multiple message.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void HandleRetrieveMultipleMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre update message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePreUpdateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post update message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostUpdateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post update asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostUpdateAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre associate message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePreAssociateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post associate message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostAssociateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post associate asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostAssociateAsyncMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity pre disassociate message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePreDisassociateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post disassociate message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostDisassociateMessage(PluginExecutionContext context)
        {
        }

        /// <summary>
        /// Handles the entity post disassociate asynchronous message.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        public virtual void HandlePostDisassociateAsyncMessage(PluginExecutionContext context)
        {
        }

        private void ExecuteCreate(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandlePreCreateMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandlePostCreateAsyncMessage(context);
                }
                else
                {
                    HandlePostCreateMessage(context);
                }
            }
            else
            {
                ThrowMessageNotFoundException(context.PluginContext.MessageName, context.PluginContext.Stage);
            }
        }

        private void ExecuteDelete(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandlePreDeleteMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandlePostDeleteAsyncMessage(context);
                }
                else
                {
                    HandlePostDeleteMessage(context);
                }
            }
            else
            {
                ThrowMessageNotFoundException(context.PluginContext.MessageName, context.PluginContext.Stage);
            }
        }

        private void ExecuteRetrieve(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandlePreRetrieveMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandlePostRetrieveAsyncMessage(context);
                }
                else
                {
                    HandlePostRetrieveMessage(context);
                }
            }
            else
            {
                HandleRetrieveMessage(context);
            }
        }

        private void ExecuteRetrieveMultiple(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandlePreRetrieveMultipleMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandlePostRetrieveMultipleAsyncMessage(context);
                }
                else
                {
                    HandlePostRetrieveMultipleMessage(context);
                }
            }
            else
            {
                HandleRetrieveMultipleMessage(context);
            }
        }

        private void ExecuteUpdate(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandlePreUpdateMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandlePostUpdateAsyncMessage(context);
                }
                else
                {
                    HandlePostUpdateMessage(context);
                }
            }
            else
            {
                ThrowMessageNotFoundException(context.PluginContext.MessageName, context.PluginContext.Stage);
            }
        }

        private void ExecuteAssociate(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandlePreAssociateMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandlePostAssociateAsyncMessage(context);
                }
                else
                {
                    HandlePostAssociateMessage(context);
                }
            }
            else
            {
                ThrowMessageNotFoundException(context.PluginContext.MessageName, context.PluginContext.Stage);
            }
        }

        private void ExecuteDisassociate(PluginExecutionContext context)
        {
            if (IsPreOperation(context))
            {
                HandlePreDisassociateMessage(context);
            }
            else if (IsPostOperation(context))
            {
                if (IsAsyncPlugin(context))
                {
                    HandlePostDisassociateAsyncMessage(context);
                }
                else
                {
                    HandlePostDisassociateMessage(context);
                }
            }
            else
            {
                ThrowMessageNotFoundException(context.PluginContext.MessageName, context.PluginContext.Stage);
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

        private void ThrowMessageNotFoundException(string messageName, int stage)
        {
            throw new InvalidPluginExecutionException($"The message '{messageName}' is not supported in stage {stage}");
        }
    }
}
