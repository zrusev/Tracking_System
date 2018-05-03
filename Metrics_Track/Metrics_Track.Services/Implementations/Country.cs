﻿namespace Metrics_Track.Services.Implementations
{
    using Metrics_Track.Data.Models;
    using Services;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    public class Country : ICountry
    {
        private readonly TrackerDbContext db;

        public Country(TrackerDbContext db)
        {
            this.db = db;
        }
        public IEnumerable<CountryDataModel> ById(int id)
        {

            var query = from trlUserCountry in this.db.TrelUserCountry
                        join tblCountry in this.db.TblCountry on trlUserCountry.IdCountry equals tblCountry.IdCountry into UserCountry_Table
                            from leftUserCountry in UserCountry_Table.DefaultIfEmpty()
                        join tblLogin in this.db.TblLogin on trlUserCountry.IdLogin equals tblLogin.IdLogin into Login_Table
                            from leftLogin in Login_Table.DefaultIfEmpty()
                        join trlCountryProcess in this.db.TrelCountryProcess on leftUserCountry.IdCountry equals trlCountryProcess.IdCountry into CountryProcess_Table
                            from leftCountryProcess in CountryProcess_Table.DefaultIfEmpty()
                        join tblProcess in this.db.TblProcess on leftCountryProcess.IdProcess equals tblProcess.IdProcess into Process_Table
                            from leftProcess in Process_Table.DefaultIfEmpty()
                        join trlProcessActivity in this.db.TrelProcessActivity on leftProcess.IdProcess equals trlProcessActivity.IdProcess into ProcessActivity_Table
                            from leftProcessActivity in ProcessActivity_Table.DefaultIfEmpty()
                        join tblActivity in this.db.TblActivity on leftProcessActivity.IdActivity equals tblActivity.IdActivity into Activity_Table
                            from leftActivity in Activity_Table.DefaultIfEmpty()
                        join trlProcessLob in this.db.TrelProcessLob on leftProcess.IdProcess equals trlProcessLob.IdProcess into ProcessLob_Table
                            from lefProcesstLob in ProcessLob_Table.DefaultIfEmpty()
                        join tblLob in this.db.TblLob on lefProcesstLob.IdLob equals tblLob.IdLob into Lob_Table
                            from leftLob in Lob_Table.DefaultIfEmpty()
                        join trlProcessStatus in this.db.TrelProcessStatus on leftProcess.IdProcess equals trlProcessStatus.IdProcess into ProcessStatus_Table
                            from leftProcessStatus in ProcessStatus_Table.DefaultIfEmpty()
                        join tbl_Status in this.db.TblStatus on leftProcessStatus.IdStatus equals tbl_Status.IdStatus into Status_Table
                            from leftStatus in Status_Table.DefaultIfEmpty()
                        join trlProcessDivision in this.db.TrelProcessDivision on leftProcess.IdProcess equals trlProcessDivision.IdProcess into ProcessDivision_Table
                             from leftProcessDivision in ProcessDivision_Table.DefaultIfEmpty()
                        join tblDivision in this.db.TblDivision on leftProcessDivision.IdDivision equals tblDivision.IdDivision into Division_Table
                             from leftDivision in Division_Table.DefaultIfEmpty()
                        join trlProcessTower in this.db.TrelProcessTower on leftProcess.IdProcess equals trlProcessTower.IdProcess into ProcessTower_Table
                            from leftProcessTower in ProcessTower_Table.DefaultIfEmpty()
                        join tblTower in this.db.TblTower on leftProcessTower.IdTower equals tblTower.IdTower into Tower_Table
                            from leftTower in Tower_Table.DefaultIfEmpty()
                        join trlProcessTowerCategory in this.db.TrelProcessTowerCategory on leftProcess.IdProcess equals trlProcessTowerCategory.IdProcess into ProcessTowerCategory_Table
                            from leftProcessTowerCategory in ProcessTowerCategory_Table.DefaultIfEmpty()
                        join tblTowerCategory in this.db.TblTowerCategory on leftProcessTowerCategory.IdTowerCategory equals tblTowerCategory.IdTowerCategory into TowerCategory_Table
                            from leftTowerCategory in TowerCategory_Table.DefaultIfEmpty()
                        where leftLogin.IdLogin == id
                        select new ProcessMap
                        {
                            IdCountry = leftUserCountry.IdCountry,
                            Country = leftUserCountry.Country,
                            IdProcess = leftProcess.IdProcess,
                            _ProcessMap = leftProcess.ProcessMap,
                            FunctionName = leftProcess.FunctionName,
                            IdLogin = leftLogin.IdLogin,
                            DisplayName = leftLogin.DisplayName,
                            IdActivity = leftActivity.IdActivity,
                            Activity = leftActivity.Activity,
                            IdLob = leftLob.IdLob,
                            Lob = leftLob.Lob,
                            IdStatus = leftStatus.IdStatus,
                            Status = leftStatus.Status,
                            IdDivision = leftProcess.IdProcess,
                            Division = leftProcess.ProcessMap,
                            IdTower = leftProcess.IdProcess,
                            Tower = leftProcess.ProcessMap,
                            IdTowerCategory = leftProcess.IdProcess,
                            TowerCategory = leftProcess.ProcessMap
                        };


            List<CountryDataModel> countries = new List<CountryDataModel>();

            foreach (var item in query)
            {
                //If a country is present check for missing children
                if (countries.Any(c => c.ID_Country.Equals(item.IdCountry)))
                {
                    int countryIndex = countries.FindIndex(c => c.ID_Country.Equals(item.IdCountry));

                    //If a process is present check for missing children
                    if (countries[countryIndex].ProcessList.Any(p => p.ID_Process.Equals(item.IdProcess)))
                    {
                        int processIndex = countries[countryIndex].ProcessList.FindIndex(i => i.ID_Process.Equals(item.IdProcess));
                        
                        //Add new LOB if missing
                        if (!countries[countryIndex].ProcessList[processIndex].LobList.Any(l => l.ID_Lob.Equals(item.IdLob)))
                        {
                            countries[countryIndex].ProcessList[processIndex].LobList.Add(new LobDataModel
                            {
                                ID_Lob = item.IdLob,
                                Lob = item.Lob
                            });
                        }

                        //Add new activity if missing
                        if (!countries[countryIndex].ProcessList[processIndex].ActivityList.Any(a => a.ID_Activity.Equals(item.IdActivity)))
                        {
                            countries[countryIndex].ProcessList[processIndex].ActivityList.Add(new ActivityDataModel
                            {
                                ID_Activity = item.IdActivity,
                                Activity = item.Activity
                            });
                        }

                        //Add new status if missing
                        if (!countries[countryIndex].ProcessList[processIndex].StatusList.Any(s => s.ID_Status.Equals(item.IdStatus)))
                        {
                            countries[countryIndex].ProcessList[processIndex].StatusList.Add(new StatusDataModel
                            {
                                ID_Status = item.IdStatus,
                                Status = item.Status
                            });
                        }
                    }
                    else
                    {
                        //Add new process if missing
                        countries[countryIndex].ProcessList.Add(new ProcessDataModel
                        {
                            ID_Process = item.IdProcess,
                            Process = item._ProcessMap
                        });
                    }
                }
                else
                {
                    //Add new country
                    countries.Add(new CountryDataModel
                    {
                        ID_Country = item.IdCountry,
                        Country = item.Country,
                        ProcessList = new List<ProcessDataModel>()
                        {
                            //Add new process
                            new ProcessDataModel
                            {
                                ID_Process = item.IdProcess,
                                Process = item._ProcessMap,
                                ActivityList =
                                    new List<ActivityDataModel>()
                                    {
                                        //Add new activity
                                        new ActivityDataModel
                                        {
                                            ID_Activity = item.IdActivity,
                                            Activity = item.Activity
                                        }
                                    },
                                LobList =
                                    new List<LobDataModel>()
                                    {
                                        //Add new lob
                                        new LobDataModel
                                        {
                                            ID_Lob = item.IdLob,
                                            Lob = item.Lob
                                        }
                                    },
                                StatusList = new List<StatusDataModel>()
                                {
                                    //Add new status
                                    new StatusDataModel
                                    {
                                        ID_Status = item.IdStatus,
                                        Status = item.Status
                                    }
                                }
                            }
                        }
                    });
                }
         }

            return countries;
        }
    }
}
