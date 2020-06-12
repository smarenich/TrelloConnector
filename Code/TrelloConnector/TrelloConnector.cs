using PX.Commerce.Core;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrelloConnector
{
	#region TrelloConnectorFactory
	public class TrelloConnectorFactory : BaseConnectorFactory<TrelloConnector>, IConnectorFactory
	{
		public override string Type => "TRC";
		public override string Description => "Trello";
		public bool Enabled => FeaturesHelper.BigCommerceConnector;

		public TrelloConnectorFactory(IEnumerable<IProcessorsFactory> processors)
			: base(processors)
		{
		}

		public IConnectorDescriptor GetDescriptor()
		{
			return new TrellosConnectorDescriptor(_processors.Values.ToList());
		}
	}
	#endregion
	#region TrelloProcessorsFactory
	public class TrelloProcessorsFactory : IProcessorsFactory
	{
		public string ConnectorType => "TRC";

		public IEnumerable<Type> GetProcessorTypes()
		{
			yield return typeof(TrelloTodoProcessor);
		}
	}
	#endregion

	public class TrelloConnector : BCConnectorBase<TrelloConnector>, IConnector
	{
		#region IConnector
		public const string TYPE = "TRC";
		public const string NAME = "Trello";

		public class trelloConnectorType : PX.Data.BQL.BqlString.Constant<trelloConnectorType>
		{
			public trelloConnectorType() : base(TYPE) { }
		}

		public override string ConnectorType { get => TYPE; }
		public override string ConnectorName { get => NAME; }

		public virtual IEnumerable<TInfo> GetExternalInfo<TInfo>(string infoType, int? bindingID)
			where TInfo : class
		{
			return null;
		}
		#endregion

		#region Navigation
		public void NavigateExtern(ISyncStatus status)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Process
		public virtual SyncInfo[] Process(ConnectorOperation operation, Int32?[] syncIDs = null)
		{
			LogInfo(operation.LogScope(), BCMessages.LogConnectorStarted, NAME);

			EntityInfo info = GetEntities().FirstOrDefault(e => e.EntityType == operation.EntityType);
			using (IProcessor graph = (IProcessor)PXGraph.CreateInstance(info.ProcessorType))
			{
				graph.Initialise(this, operation);
				return graph.Process(syncIDs);
			}
		}

		public DateTime GetSyncTime(ConnectorOperation operation)
		{
			//Acumatica Time
			PXDatabase.SelectDate(out DateTime dtLocal, out DateTime dtUtc);
			dtLocal = PX.Common.PXTimeZoneInfo.ConvertTimeFromUtc(dtUtc, PX.Common.LocaleInfo.GetTimeZone());

			return dtLocal;
		}
		#endregion

		#region Notifications
		public override void StartWebHook(BCWebHook hook)
		{
			throw new NotImplementedException();
		}
		public override void StopWebHook(BCWebHook hook)
		{
			throw new NotImplementedException();
		}

		public virtual void ProcessHook(IEnumerable<BCExternQueueMessage> messages)
		{
			throw new NotImplementedException();
		}
		#endregion
	}

	#region TrellosConnectorDescriptor
	public class TrellosConnectorDescriptor : IConnectorDescriptor
	{
		protected IList<EntityInfo> _entities;

		public TrellosConnectorDescriptor(IList<EntityInfo> entities)
		{
			_entities = entities;
		}

		public virtual Guid? GenerateExternID(BCExternNotification message)
		{
			throw new NotImplementedException();
		}
		public virtual Guid? GenerateLocalID(BCLocalNotification message)
		{
			throw new NotImplementedException();
		}
		public List<Tuple<string, string, string>> GetExternalFields(string type, int? binding, string entity)
		{
			throw new NotImplementedException();
		}
	}
	#endregion
}
