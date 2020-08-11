using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace MikeFactorial.Xrm.Plugins.DataProviders
{
    /// <summary>
    /// Contains the context elements sent from CDS for a plugin.
    /// </summary>
    /// <seealso cref="MikeFactorial.Xrm.Plugins.BaseExecutionContext" />
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Microsoft.Xrm.Sdk.ITracingService" />
    public sealed class PluginExecutionContext : BaseExecutionContext, IDisposable, ITracingService
    {
        private Entity completeEntity;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginExecutionContext"/> class.
        /// </summary>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="context">The context.</param>
        /// <param name="trace">The trace.</param>
        /// <param name="secureConfig">The secure configuration.</param>
        /// <param name="unsecureConfig">The unsecure configuration.</param>
        public PluginExecutionContext(IOrganizationService organizationService, IPluginExecutionContext context, ITracingService trace, string secureConfig, string unsecureConfig)
        {
            SecureConfig = secureConfig;
            UnsecureConfig = unsecureConfig;
            TracingService = trace;
            Service = organizationService;
            Context = context;
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the PluginExecutionContext class. Constructor to be used from a Microsoft Dynamics CRM (365) plugin
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider passed to the IPlugin.Execute method</param>
        /// <param name="secureConfig">Secure Configuaration</param>
        /// <param name="unsecureConfig">Unsecure Configuaration</param>
        public PluginExecutionContext(IServiceProvider serviceProvider, string secureConfig, string unsecureConfig)
        {
            ServiceProvider = serviceProvider;
            SecureConfig = secureConfig;
            UnsecureConfig = unsecureConfig;
            TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            Service = serviceFactory.CreateOrganizationService(Context.InitiatingUserId);
            Init();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the secure configuration.
        /// </summary>
        /// <value>
        /// The secure configuration.
        /// </value>
        public string SecureConfig { get; set; }

        /// <summary>
        /// Gets or sets the unsecure configuration.
        /// </summary>
        /// <value>
        /// The unsecure configuration.
        /// </value>
        public string UnsecureConfig { get; set; }

        /// <summary>
        /// Gets the plugin context.
        /// </summary>
        /// <value>
        /// The plugin context.
        /// </value>
        public IPluginExecutionContext PluginContext => Context as IPluginExecutionContext;

        /// <summary>
        /// Gets the pre image entity from the plugin execution context.
        /// </summary>
        /// <value>
        /// The pre image entity.
        /// </value>
        public Entity PreImage
        {
            get
            {
                if (Context != null &&
                    Context.PreEntityImages != null &&
                    Context.PreEntityImages.Count > 0)
                {
                    return Context.PreEntityImages.Values.FirstOrDefault();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the post image entity from the plugin execution context.
        /// </summary>
        /// <value>
        /// The post image entity.
        /// </value>
        public Entity PostImage
        {
            get
            {
                if (Context != null &&
                    Context.PostEntityImages != null &&
                    Context.PostEntityImages.Count > 0)
                {
                    return Context.PostEntityImages.Values.FirstOrDefault();
                }

                return null;
            }
        }

        #endregion Public properties

        #region Public methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Method intentionally left empty.
        }

        #endregion Public methods

        private bool AreEqual(object value1, object value2)
        {
            return ((value1 != null) && value1.GetType().IsValueType)
                                 ? value1.Equals(value2)
                                 : (value1 == value2);
        }
    }
}
