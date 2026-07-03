using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.SqlDataSeed
{
    public class DataSeedRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
    public class DataSeedRoleList
    {
        public DataSeedRole rol_Vendedor = new DataSeedRole
        {
            Id = "d2716b4b-671d-4814-a04c-b51aa97051e8",
            Name = "Vendedor",
            NormalizedName = "VENDEDOR"
        };
        public DataSeedRole rol_Desarrollador = new DataSeedRole
        {
            Id = "5a233184-1b23-4a52-a24f-054ee5ae2cf6",
            Name = "Desarrollador",
            NormalizedName = "DESARROLLADOR"
        };
        public DataSeedRole rol_Clinica = new DataSeedRole
        {
            Id = "126dfad6-796e-4d87-915b-68ae732dc4e4",
            Name = "Clinica",
            NormalizedName = "CLINICA"
        };
        public DataSeedRole rol_RecepcionDiurno = new DataSeedRole
        {
            Id = "126dfad6-796e-4d87-915b-68ae732dc4e6",
            Name = "Recepcion Diurno",
            NormalizedName = "RECEPCION DIURNO"
        };
        public DataSeedRole rol_RecepcionNocturno = new DataSeedRole
        {
            Id = "126dfad6-796e-4d87-915b-68ae732dc4e9",
            Name = "Recepcion Nocturno",
            NormalizedName = "RECEPCION NOCTURNO"
        };
        public DataSeedRole rol_Enfermeria = new DataSeedRole
        {
            Id = "126dfad6-796e-4d87-915b-68ae732dc4e8",
            Name = "Enfermeria",
            NormalizedName = "ENFERMERIA"
        };
        public DataSeedRole rol_Mensajero = new DataSeedRole
        {
            Id = "4e725ea5-282e-401c-9e82-da4947e382ef",
            Name = "Mensajero",
            NormalizedName = "MENSAJERO"
        };
        public DataSeedRole rol_Supervisor = new DataSeedRole
        {
            Id = "3c06eaf3-10d4-40ae-bd00-3d50d629cfde",
            Name = "Supervisor",
            NormalizedName = "SUPERVISOR"
        };
        public DataSeedRole rol_Farmacia = new DataSeedRole
        {
            Id = "123dfad6-796e-4d87-915b-68ae732dc4e1",
            Name = "Farmacia",
            NormalizedName = "FARMACIA"
        };
        public DataSeedRole rol_Hospital = new DataSeedRole
        {
            Id = "125dfad6-796e-4d87-915b-68ae732dc4e3",
            Name = "Hospital",
            NormalizedName = "HOSPITAL"
        };
        public DataSeedRole rol_Coordinacion = new DataSeedRole
        {
            Id = "126dfad6-796e-4d87-915b-68ae732dc4e5",
            Name = "Coordinacion",
            NormalizedName = "COORDINACION"
        };
        public DataSeedRole rol_Cajero = new DataSeedRole
        {
            Id = "ec320394-8501-4710-a2ae-85e04315a5f9",
            Name = "Cajero",
            NormalizedName = "CAJERO"
        };
        public DataSeedRole rol_Administrador = new DataSeedRole
        {
            Id = "785c160e-6a4f-4fd8-8121-4e6b2af230cd",
            Name = "Administrador",
            NormalizedName = "ADMINISTRADOR"
        };
        public DataSeedRole rol_TecnicoLaboratorio = new DataSeedRole
        {
            Id = "124dfad6-796e-4d87-915b-68ae732dc4e2",
            Name = "Tecnico laboratorio",
            NormalizedName = "TECNICO LABORATORIO"
        };
        public DataSeedRole rol_SupervisorLaboratorio = new DataSeedRole
        {
            Id = "737ff76f-c976-4ba3-bdb9-5b19fe11bbca",
            Name = "Supervisor Laboratorio",
            NormalizedName = "SUPERVISOR LABORATORIO"
        };
        public DataSeedRole rol_Urologia = new DataSeedRole
        {
            Id = "497fdf79-d5c2-4ed9-80a8-13cee0f86b39",
            Name = "Urologia",
            NormalizedName = "UROLOGIA"
        };
        public DataSeedRole rol_MedicoInterno = new DataSeedRole
        {
            Id = "725dfad6-796e-4d87-915b-68ae732dc4e7",
            Name = "Medico Interno",
            NormalizedName = "MEDICO INTERNO"
        };
        public DataSeedRole rol_MedicoExterno = new DataSeedRole
        {
            Id = "cd550174-2cbe-41a7-a67b-670dfcd9d49c",
            Name = "Medico Externo",
            NormalizedName = "MEDICO EXTERNO"
        };
        public DataSeedRole rol_MedicoGeneral = new DataSeedRole
        {
            Id = "84c54e1d-eb93-4759-ab90-d5d7007c5e55",
            Name = "Medico General",
            NormalizedName = "MEDICO GENERAL"
        };
        public DataSeedRole rol_JefaturaMedica = new DataSeedRole
        {
            Id = "0263c5c9-4a7d-44f8-bee1-51529c17d0bc",
            Name = "Jefatura Medica",
            NormalizedName = "JEFATURA MEDICA"
        };

        public DataSeedRoleList()
        {
            RoleList = new List<DataSeedRole>();
            RoleList.Add(rol_Vendedor);
            RoleList.Add(rol_Desarrollador);
            RoleList.Add(rol_Clinica);
            RoleList.Add(rol_RecepcionDiurno);
            RoleList.Add(rol_RecepcionNocturno);
            RoleList.Add(rol_Enfermeria);
            RoleList.Add(rol_Mensajero);
            RoleList.Add(rol_Supervisor);
            RoleList.Add(rol_Farmacia);
            RoleList.Add(rol_Hospital);
            RoleList.Add(rol_Coordinacion);
            RoleList.Add(rol_Cajero);
            RoleList.Add(rol_Administrador);
            RoleList.Add(rol_TecnicoLaboratorio);
            RoleList.Add(rol_SupervisorLaboratorio);
            RoleList.Add(rol_Urologia);
            RoleList.Add(rol_MedicoInterno);
            RoleList.Add(rol_MedicoExterno);
            RoleList.Add(rol_MedicoGeneral);
            RoleList.Add(rol_JefaturaMedica);
        }
        public List<DataSeedRole> RoleList { get; set; }
    }
}
