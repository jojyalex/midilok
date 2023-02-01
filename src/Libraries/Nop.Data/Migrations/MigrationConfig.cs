using System.Globalization;

namespace Nop.Data.Migrations;

public partial class MigrationConfig
{
    #region Fields

    protected long? _version;
    protected string _description;

    #endregion

    /// <summary>
    /// Gets or sets the migration date time string to convert on version
    /// </summary>
    public string DateTime { get; set; }

    /// <summary>
    /// nopCommerce full version
    /// </summary>
    public string NopVersion { get; set; }

    /// <summary>
    /// Gets or sets the update migration type
    /// </summary>
    public virtual UpdateMigrationType? UpdateMigrationType { get; set; }

    /// <summary>
    /// Gets or sets the target migration process type
    /// </summary>
    public virtual MigrationProcessType TargetMigrationProcess { get; set; } = MigrationProcessType.NoMatter;

    /// <summary>
    /// Gets or sets the migration version
    /// </summary>
    public virtual long Version {
        get
        {
            if (_version.HasValue)
                return _version.Value;

            //TODO: add checking DateTime
            var version = System.DateTime
                .ParseExact(DateTime, NopMigrationDefaults.DateFormats, CultureInfo.InvariantCulture).Ticks;

            if (UpdateMigrationType.HasValue)
                version += (int)UpdateMigrationType;

            return version;
        }
        set => _version = value;
    }

    /// <summary>
    /// Gets or sets the migration description
    /// </summary>
    public virtual string Description
    {
        get
        {
            if (!string.IsNullOrEmpty(_description))
                return _description;

            string description = null;

            //TODO: add checking NopVersion
            if (UpdateMigrationType.HasValue)
                description = string.Format(NopMigrationDefaults.UpdateMigrationDescription, NopVersion, UpdateMigrationType.ToString());

            return description;
        }
        set => _description = value;
    }

    /// <summary>
    /// Gets the flag which indicate whether the migration should be applied into DB on the debug mode
    /// </summary>
    public bool ApplyInDbOnDebugMode { get; set; } = true;

    /// <summary>
    /// Gets or sets the value which indicate is this schema migration
    /// </summary>
    ///<remarks>
    /// If set to true than this migration will apply right after the migration runner will become available.
    /// Do not us dependency injection in migrations that are marked as schema migration,
    /// because IoC container not ready yet.
    ///</remarks>
    public virtual bool IsSchemaMigration { get; set; } = false;
}