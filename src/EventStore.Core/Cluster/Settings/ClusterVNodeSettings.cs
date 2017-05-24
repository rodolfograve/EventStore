using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using EventStore.Common.Utils;
using EventStore.Core.Authentication;
using EventStore.Core.Data;
using EventStore.Core.Services.Monitoring;
using EventStore.Core.Services.PersistentSubscription.ConsumerStrategy;

namespace EventStore.Core.Cluster.Settings
{
    public class ClusterVNodeSettings
    {
        public readonly VNodeInfo NodeInfo;
        public readonly GossipAdvertiseInfo GossipAdvertiseInfo;
        public readonly string[] IntHttpPrefixes;
        public readonly string[] ExtHttpPrefixes;
        public readonly bool EnableTrustedAuth;
        public readonly X509Certificate2 Certificate;
        public readonly int WorkerThreads;
        public readonly bool StartStandardProjections;
        public readonly bool DisableHTTPCaching;
        public readonly bool LogHttpRequests;

        public readonly bool DiscoverViaDns;
        public readonly string ClusterDns;
        public readonly IPEndPoint[] GossipSeeds;
        public readonly bool EnableHistograms;
        public readonly TimeSpan MinFlushDelay;

        public readonly int ClusterNodeCount;
        public readonly int PrepareAckCount;
        public readonly int CommitAckCount;
        public readonly TimeSpan PrepareTimeout;
        public readonly TimeSpan CommitTimeout;

        public readonly int NodePriority;

        public readonly bool UseSsl;
        public readonly bool DisableInsecureTCP;
        public readonly string SslTargetHost;
        public readonly bool SslValidateServer;

        public readonly TimeSpan StatsPeriod;
        public readonly StatsStorage StatsStorage;

        public readonly IAuthenticationProviderFactory AuthenticationProviderFactory;
        public readonly bool DisableScavengeMerging;
        public readonly int ScavengeHistoryMaxAge;
        public bool AdminOnPublic;
        public bool StatsOnPublic;
        public bool GossipOnPublic;
        public readonly TimeSpan GossipInterval;
        public readonly TimeSpan GossipAllowedTimeDifference;
        public readonly TimeSpan GossipTimeout;
        public readonly TimeSpan IntTcpHeartbeatTimeout;
        public readonly TimeSpan IntTcpHeartbeatInterval;
        public readonly TimeSpan ExtTcpHeartbeatTimeout;
        public readonly TimeSpan ExtTcpHeartbeatInterval;
        public readonly bool UnsafeIgnoreHardDeletes;
        public readonly bool VerifyDbHash;
        public readonly int MaxMemtableEntryCount;
        public readonly int HashCollisionReadLimit;
        public readonly int IndexCacheDepth;
        public readonly byte IndexBitnessVersion;

        public readonly bool BetterOrdering;
        public readonly string Index;
        public readonly int ReaderThreadsCount;
        public readonly IPersistentSubscriptionConsumerStrategyFactory[] AdditionalConsumerStrategies;
        public readonly bool AlwaysKeepScavenged;

        public readonly bool GossipOnSingleNode;

        public ClusterVNodeSettings(Guid instanceId, int debugIndex,
                                    IPEndPoint internalTcpEndPoint,
                                    IPEndPoint internalSecureTcpEndPoint,
                                    IPEndPoint externalTcpEndPoint,
                                    IPEndPoint externalSecureTcpEndPoint,
                                    IPEndPoint internalHttpEndPoint,
                                    IPEndPoint externalHttpEndPoint,
                                    GossipAdvertiseInfo gossipAdvertiseInfo,
                                    string[] intHttpPrefixes,
                                    string[] extHttpPrefixes,
                                    bool enableTrustedAuth,
                                    X509Certificate2 certificate,
                                    int workerThreads,
                                    bool discoverViaDns,
                                    string clusterDns,
                                    IPEndPoint[] gossipSeeds,
                                    TimeSpan minFlushDelay,
                                    int clusterNodeCount,
                                    int prepareAckCount,
                                    int commitAckCount,
                                    TimeSpan prepareTimeout,
                                    TimeSpan commitTimeout,
                                    bool useSsl,
                                    bool disableInsecureTCP,
                                    string sslTargetHost,
                                    bool sslValidateServer,
                                    TimeSpan statsPeriod,
                                    StatsStorage statsStorage,
                                    int nodePriority,
                                    IAuthenticationProviderFactory authenticationProviderFactory,
                                    bool disableScavengeMerging,
                                    int scavengeHistoryMaxAge,
                                    bool adminOnPublic,
                                    bool statsOnPublic,
                                    bool gossipOnPublic,
                                    TimeSpan gossipInterval,
                                    TimeSpan gossipAllowedTimeDifference,
                                    TimeSpan gossipTimeout,
                                    TimeSpan intTcpHeartbeatTimeout,
                                    TimeSpan intTcpHeartbeatInterval,
                                    TimeSpan extTcpHeartbeatTimeout,
                                    TimeSpan extTcpHeartbeatInterval,
				                    bool verifyDbHash,
				                    int maxMemtableEntryCount,
                                    int hashCollisionReadLimit,
                                    bool startStandardProjections,
                                    bool disableHTTPCaching,
                                    bool logHttpRequests,
                                    string index = null, bool enableHistograms = false,
                                    int indexCacheDepth = 16,
                                    byte indexBitnessVersion = 2,
                                    IPersistentSubscriptionConsumerStrategyFactory[] additionalConsumerStrategies = null,
                                    bool unsafeIgnoreHardDeletes = false,
                                    bool betterOrdering = false,
                                    int readerThreadsCount = 4,
                                    bool alwaysKeepScavenged = false,
                                    bool gossipOnSingleNode = false)
        {
            Ensure.NotEmptyGuid(instanceId, "instanceId");
            Ensure.NotNull(internalTcpEndPoint, "internalTcpEndPoint");
            Ensure.NotNull(externalTcpEndPoint, "externalTcpEndPoint");
            Ensure.NotNull(internalHttpEndPoint, "internalHttpEndPoint");
            Ensure.NotNull(externalHttpEndPoint, "externalHttpEndPoint");
            Ensure.NotNull(intHttpPrefixes, "intHttpPrefixes");
            Ensure.NotNull(extHttpPrefixes, "extHttpPrefixes");
            if (internalSecureTcpEndPoint != null || externalSecureTcpEndPoint != null)
                Ensure.NotNull(certificate, "certificate");
            Ensure.Positive(workerThreads, "workerThreads");
            Ensure.NotNull(clusterDns, "clusterDns");
            Ensure.NotNull(gossipSeeds, "gossipSeeds");
            Ensure.Positive(clusterNodeCount, "clusterNodeCount");
            Ensure.Positive(prepareAckCount, "prepareAckCount");
            Ensure.Positive(commitAckCount, "commitAckCount");
            Ensure.NotNull(gossipAdvertiseInfo, "gossipAdvertiseInfo");

            if (discoverViaDns && string.IsNullOrWhiteSpace(clusterDns))
                throw new ArgumentException("Either DNS Discovery must be disabled (and seeds specified), or a cluster DNS name must be provided.");

            if (useSsl)
                Ensure.NotNull(sslTargetHost, "sslTargetHost");

            NodeInfo = new VNodeInfo(instanceId, debugIndex,
                                     internalTcpEndPoint, internalSecureTcpEndPoint,
                                     externalTcpEndPoint, externalSecureTcpEndPoint,
                                     internalHttpEndPoint, externalHttpEndPoint);
            GossipAdvertiseInfo = gossipAdvertiseInfo;
            IntHttpPrefixes = intHttpPrefixes;
            ExtHttpPrefixes = extHttpPrefixes;
            EnableTrustedAuth = enableTrustedAuth;
            Certificate = certificate;
            WorkerThreads = workerThreads;
            StartStandardProjections = startStandardProjections;
            DisableHTTPCaching = disableHTTPCaching;
            LogHttpRequests = logHttpRequests;
            AdditionalConsumerStrategies = additionalConsumerStrategies ?? new IPersistentSubscriptionConsumerStrategyFactory[0];

            DiscoverViaDns = discoverViaDns;
            ClusterDns = clusterDns;
            GossipSeeds = gossipSeeds;
            GossipOnSingleNode = gossipOnSingleNode;

            ClusterNodeCount = clusterNodeCount;
            MinFlushDelay = minFlushDelay;
            PrepareAckCount = prepareAckCount;
            CommitAckCount = commitAckCount;
            PrepareTimeout = prepareTimeout;
            CommitTimeout = commitTimeout;

            UseSsl = useSsl;
            DisableInsecureTCP = disableInsecureTCP;
            SslTargetHost = sslTargetHost;
            SslValidateServer = sslValidateServer;

            StatsPeriod = statsPeriod;
            StatsStorage = statsStorage;

            AuthenticationProviderFactory = authenticationProviderFactory;

            NodePriority = nodePriority;
            DisableScavengeMerging = disableScavengeMerging;
            ScavengeHistoryMaxAge = scavengeHistoryMaxAge;
            AdminOnPublic = adminOnPublic;
            StatsOnPublic = statsOnPublic;
            GossipOnPublic = gossipOnPublic;
            GossipInterval = gossipInterval;
            GossipAllowedTimeDifference = gossipAllowedTimeDifference;
            GossipTimeout = gossipTimeout;
            IntTcpHeartbeatTimeout = intTcpHeartbeatTimeout;
            IntTcpHeartbeatInterval = intTcpHeartbeatInterval;
            ExtTcpHeartbeatTimeout = extTcpHeartbeatTimeout;
            ExtTcpHeartbeatInterval = extTcpHeartbeatInterval;

            VerifyDbHash = verifyDbHash;
            MaxMemtableEntryCount = maxMemtableEntryCount;
            HashCollisionReadLimit = hashCollisionReadLimit;

            EnableHistograms = enableHistograms;
            IndexCacheDepth = indexCacheDepth;
            IndexBitnessVersion = indexBitnessVersion;
            Index = index;
            UnsafeIgnoreHardDeletes = unsafeIgnoreHardDeletes;
            BetterOrdering = betterOrdering;
            ReaderThreadsCount = readerThreadsCount;
            AlwaysKeepScavenged = alwaysKeepScavenged;
        }


        public override string ToString() =>
            $"{nameof(NodeInfo.InstanceId)}: {NodeInfo.InstanceId}\n"
            + $"{nameof(NodeInfo.InternalTcp)}: {NodeInfo.InternalTcp}\n"
            + $"{nameof(NodeInfo.InternalSecureTcp)}: {NodeInfo.InternalSecureTcp}\n"
            + $"{nameof(NodeInfo.ExternalTcp)}: {NodeInfo.ExternalTcp}\n"
            + $"{nameof(NodeInfo.ExternalSecureTcp)}: {NodeInfo.ExternalSecureTcp}\n"
            + $"{nameof(NodeInfo.InternalHttp)}: {NodeInfo.InternalHttp}\n"
            + $"{nameof(NodeInfo.ExternalHttp)}: {NodeInfo.ExternalHttp}\n"
            + $"{nameof(IntHttpPrefixes)}: {string.Join(", ", IntHttpPrefixes)}\n"
            + $"{nameof(ExtHttpPrefixes)}: {string.Join(", ", ExtHttpPrefixes)}\n"
            + $"{nameof(EnableTrustedAuth)}: {EnableTrustedAuth}\n"
            + $"{nameof(Certificate)}: {Certificate?.ToString(true) ?? "n/a"}\n"
            + $"{nameof(LogHttpRequests)}: {LogHttpRequests}\n"
            + $"{nameof(WorkerThreads)}: {WorkerThreads}\n"
            + $"{nameof(DiscoverViaDns)}: {DiscoverViaDns}\n"
            + $"{nameof(ClusterDns)}: {ClusterDns}\n"
            + $"{nameof(GossipSeeds)}: {string.Join(",", GossipSeeds.Select(x => x.ToString()))}\n"
            + $"{nameof(ClusterNodeCount)}: {ClusterNodeCount}\n"
            + $"{nameof(MinFlushDelay)}: {MinFlushDelay}\n"
            + $"{nameof(PrepareAckCount)}: {PrepareAckCount}\n"
            + $"{nameof(CommitAckCount)}: {CommitAckCount}\n"
            + $"{nameof(PrepareTimeout)}: {PrepareTimeout}\n"
            + $"{nameof(CommitTimeout)}: {CommitTimeout}\n"
            + $"{nameof(UseSsl)}: {UseSsl}\n"
            + $"{nameof(SslTargetHost)}: {SslTargetHost}\n"
            + $"{nameof(SslValidateServer)}: {SslValidateServer}\n"
            + $"{nameof(StatsPeriod)}: {StatsPeriod}\n"
            + $"{nameof(StatsStorage)}: {StatsStorage}\n"
            + $"{nameof(AuthenticationProviderFactory)} Type: {AuthenticationProviderFactory.GetType()}\n"
            + $"{nameof(NodePriority)}: {NodePriority}"
            + $"{nameof(GossipInterval)}: {GossipInterval}\n"
            + $"{nameof(GossipAllowedTimeDifference)}: {GossipAllowedTimeDifference}\n"
            + $"{nameof(GossipTimeout)}: {GossipTimeout}\n"
            + $"{nameof(EnableHistograms)}: {EnableHistograms}\n"
            + $"{nameof(DisableHTTPCaching)}: {DisableHTTPCaching}\n"
            + $"{nameof(Index)}: {Index}\n"
            + $"{nameof(ScavengeHistoryMaxAge)}: {ScavengeHistoryMaxAge}\n";
    }
}
