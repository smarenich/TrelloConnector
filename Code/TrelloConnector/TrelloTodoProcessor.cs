using Newtonsoft.Json;
using PX.Commerce.Core;
using PX.Data;
using PX.Objects.CR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrelloConnector
{
	#region MappedPayment
	public class MappedCase : MappedEntity<CardData, Case>
	{
		public const String TYPE = "CD";

		public MappedCase()
			: base("TRC", TYPE)
		{ }
		public MappedCase(Case entity, Guid? id, DateTime? timestamp)
			: base("TRC", TYPE, entity, id, timestamp) { }
		public MappedCase(CardData entity, String id, DateTime? timestamp)
			: base("TRC", TYPE, entity, id, timestamp) { }
	}
	#endregion

	public class TrelloCaseEntityBucket : EntityBucketBase, IEntityBucket
	{
		public IMappedEntity Primary => Case;
		public IMappedEntity[] Entities => new IMappedEntity[] { Primary };

		public MappedCase Case;
	}


	[BCProcessor(typeof(TrelloConnector), "CD", "Card",
		IsInternal = false,
		Direction = SyncDirection.Import,
		PrimaryDirection = SyncDirection.Import,
		PrimarySystem = PrimarySystem.Extern,
		PrimaryGraph = typeof(PX.Objects.CR.CRCaseMaint),
		AcumaticaPrimaryType = typeof(PX.Objects.CR.CRCase),
		ExternTypes = new Type[] { typeof(CardData) },
		LocalTypes = new Type[] { typeof(Case) })]
	[BCProcessorRealtime(PushSupported = false, HookSupported = false)]
	public class TrelloTodoProcessor : BCProcessorSingleBase<TrelloTodoProcessor, TrelloCaseEntityBucket, MappedCase>, IProcessor
	{
		public TrelloRestClient Client;

		#region Constructor
		public override void Initialise(IConnector iconnector, ConnectorOperation operation)
		{
			base.Initialise(iconnector, operation);

			JsonSerializer serializer = new JsonSerializer
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.Ignore,
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
			};
			RestJsonSerializer restSerializer = new RestJsonSerializer(serializer);
			Client = new TrelloRestClient("https://api.trello.com/1", restSerializer, restSerializer);
		}
		#endregion

		#region Import
		public override void GetBucketsForImport(DateTime? lastModifiedDateTime, PXFilterRow[] filters)
		{
			IEnumerable<CardData> cards = Client.Get<List<CardData>>(
				"/lists/571d8ed34283b465a5798e24/cards?key=<>&token=<>");

			foreach (CardData data in cards)
			{
				TrelloCaseEntityBucket bucket = CreateBucket();

				MappedCase mapped = bucket.Case = bucket.Case.Set(data, data.Id, data.LastActivity);
				EntityStatus status = EnsureStatus(mapped, SyncDirection.Import);
			}
		}
		public override bool GetBucketForImport(TrelloCaseEntityBucket bucket, BCSyncStatus syncstatus)
		{
			CardData card = Client.Get<CardData>(
				String.Format("/cards/{0}?key=<>f&token=<>", syncstatus.ExternID));

			MappedCase obj = bucket.Case = bucket.Case.Set(card, card.Id, card.LastActivity);
			EntityStatus status = EnsureStatus(obj, SyncDirection.Import);

			return status == EntityStatus.Pending;
		}

		public override void MapBucketImport(TrelloCaseEntityBucket bucket, IMappedEntity existing)
		{
			MappedCase obj = bucket.Case;

			CardData data = obj.Extern;
			Case impl = obj.Local = new Case();

			//Product
			impl.Subject = data.Name.ValueField();
			impl.Description = data.Description.ValueField();
		}
		public override void SaveBucketImport(TrelloCaseEntityBucket bucket, IMappedEntity existing, String operation)
		{
			MappedCase obj = bucket.Case;

			Case impl = cbapi.Put<Case>(obj.Local, obj.LocalID);

			bucket.Case.AddLocal(impl, impl.SyncID, impl.SyncTime);
			UpdateStatus(obj, operation);
		}
		#endregion

		#region Export
		public override void GetBucketsForExport(DateTime? lastModifiedDateTime, PXFilterRow[] filters)
		{
		}
		public override bool GetBucketForExport(TrelloCaseEntityBucket bucket, BCSyncStatus syncstatus)
		{
			return false;
		}

		public override void MapBucketExport(TrelloCaseEntityBucket bucket, IMappedEntity existing)
		{
		}
		public override void SaveBucketExport(TrelloCaseEntityBucket bucket, IMappedEntity existing, String operation)
		{
		}
		#endregion

		#region Pull
		public override MappedCase PullEntity(Guid? localID, Dictionary<string, object> externalInfo)
		{
			throw new NotImplementedException();
		}
		public override MappedCase PullEntity(string externID, string externalInfo)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
