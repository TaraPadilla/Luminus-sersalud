using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Database.Shared.SqlDataSeed
{
    public class SqlQueriesContext
    {
        public string Sp_consultas_productos_existentes_venta
        {
            get
            {
                return @"
					CREATE OR REPLACE FUNCTION public.consultas_productos_existentes_venta(
						ambienteid integer,
						sucursalid integer)
						RETURNS TABLE(productoid integer, productoinventarioid integer, productocodigo text, productonombre text, productoactivoconcentracion text, productopresentacion text, productoviaadministracion text, productogrupoterapeutico text, productolaboratorio text, productoimagen text, unidadmedidaventaid integer, unidadmedidaventanombre text, productostock numeric, precioid integer, precionombre text, preciovalor numeric) 
						LANGUAGE 'plpgsql'
						COST 100
						VOLATILE PARALLEL UNSAFE
						ROWS 1000

					AS $BODY$

									BEGIN
										RETURN QUERY
										SELECT 
											p.""Id"" AS ""ProductoId"",
											pi.""Id"" AS ""ProductoInventarioId"",
											p.""CodigoReferencia"" AS ""ProductoCodigo"",
											p.""NombreProducto"" AS ""ProductoNombre"",
											'' AS ""ProductoActivoConcentracion"", -- Ejemplo de valor predeterminado
											'' AS ""ProductoPresentacion"",
											'' AS ""ProductoViaAdministracion"",
											'' AS ""ProductoGrupoTerapeutico"",
											'' AS ""ProductoLaboratorio"",
											'' AS ""ProductoImagen"",
											pi.""UnidadVentaId"" AS ""UnidadMedidaVentaId"",
											pi.""NombreUnidadVenta"" AS ""UnidadMedidaVentaNombre"", 
											pi.""Stock"" AS ""ProductoStock"",
											0 AS ""PrecioId"", -- Ejemplo de valor predeterminado numérico
											'' AS ""PrecioNombre"",
											0.00 AS ""PrecioValor""
										FROM 
											""Productos"" AS p
										LEFT JOIN LATERAL (
											SELECT 
												pi.""Id"",
												pi.""Stock"",
												uv.""Nombre"" AS ""NombreUnidadVenta"",
												uv.""Id"" AS ""UnidadVentaId""
											FROM 
												""ProductosInventario"" AS pi
											JOIN ""UnidadesMedidaVenta"" AS uv ON pi.""UnidadMedidaVentaId"" = uv.""Id""
											JOIN ""Bodegas"" AS b ON pi.""BodegaId"" = b.""Id""
											JOIN ""Ambientes"" AS am ON b.""AmbienteId"" = am.""Id""
											JOIN ""Sucursales"" AS s ON b.""SucursalId"" = s.""Id""
											WHERE 
												pi.""ProductoId"" = p.""Id"" AND
												am.""Id"" = ambienteId AND s.""Id"" = sucursalId
											LIMIT 1
										) pi ON true
										WHERE
											pi.""Id"" IS NOT NULL;
									END;
                
					$BODY$;
";

            }
        }
        public string Sp_registrar_auditoria_producto
        {
            get
            {
                return @"CREATE OR REPLACE FUNCTION registrar_auditoria_producto(
	                            productos jsonb[],
	                            userid text,
	                            actualizostock boolean,
	                            personacreacionauditoria text)
                                RETURNS void
                                LANGUAGE 'plpgsql'
                                COST 100
                                VOLATILE PARALLEL UNSAFE
                            AS $BODY$
		                            DECLARE
			                            row jsonb;
			                            auditoria_id INTEGER;
		                            BEGIN

			                            INSERT INTO ""Auditoria""(
				                            ""FechaCreacion"",
				                            ""UserId"",
				                            ""ActualizoStock"", 
				                            ""Eliminada"", 
				                            ""PersonaCreacionAuditoria"")
			                            VALUES (CURRENT_TIMESTAMP AT TIME ZONE 'America/Guatemala', 
					                            UserId,
					                            ActualizoStock,
					                            false,
					                            PersonaCreacionAuditoria)
			                            RETURNING ""Id"" INTO auditoria_id;


			                            --Se recorre el Array de productos
			                            FOR row IN SELECT * FROM unnest(productos) LOOP
				                            INSERT INTO public.""AuditoriaProducto""(
					                            ""Stock"", ""ProductoId"", ""AuditoriaId"")
				                            VALUES ((row->>'stockingresado')::INT,
						                            (row->>'idproducto')::INT, auditoria_id);
						
											--Actualizacion de fecha de vencimiento, lote y fecha de recepcion de lote
											UPDATE ""ProductosInventario""
											SET ""FechaVencimientoArticuloCompra"" = (SELECT row->>'fecha_vencimiento')::TIMESTAMP,
											""Lote"" = (SELECT row->>'lote_producto'),
											""FechaRecepcionLote"" = (SELECT row->>'fecha_recepcion_lote')::TIMESTAMP
											WHERE ""Id"" = (SELECT row->>'id_producto_inventario')::INT;
						
											--Actualizacion de nombre y codigo de referencia
											UPDATE ""Productos""
											SET ""NombreProducto"" = (SELECT row->>'nombre_producto'),
											""CodigoReferencia"" = (SELECT row->>'codigo_referencia')
											WHERE ""Id"" = (SELECT row->>'idproducto')::INT;


				                            IF actualizostock = true THEN
					                            UPDATE ""ProductosInventario""
					                            SET ""Stock"" = (SELECT row->>'stockingresado')::INT
					                            WHERE ""Id"" = (SELECT row->>'id_producto_inventario')::INT;
				                            ELSE
				
				                            END IF;
			                            END LOOP;

		                            END;
                    
					
                            $BODY$;";
            }
        }
        public string Sp_get_inventario
        {
            get
            {
                return @"

                CREATE OR REPLACE FUNCTION get_inventario(
	                        tipo_producto_id integer,
	                        grupo_terapeutico_id integer,
	                        bodega_id integer,
	                        sucursal_id integer,
	                        ambiente_id integer)
                            RETURNS TABLE(item bigint, productoid integer, productoinventarioid integer, productonombre text, productocodigo text, productodescripcion text,ProductoActivoConcentracion text, productoimagen text, stock numeric, bodegaid integer, bodeganombre text, unidadmedidaventaid integer, unidadmedidaventanombre text, precioid integer, precionombre text, preciovalor numeric, preciocompra numeric) 
                            LANGUAGE 'plpgsql'
                            COST 100
                            VOLATILE PARALLEL UNSAFE
                            ROWS 1000

                        AS $BODY$
                                BEGIN
                                    RETURN QUERY
                                    SELECT 		ROW_NUMBER() OVER () AS ""Item""
                                                ,""A"".""Id"" AS ""ProductoId""
			                                    ,""B"".""Id"" AS ""ProductoInventarioId""
			                                    ,""A"".""NombreProducto"" AS ""ProductoNombre""
                                                ,""A"".""CodigoReferencia"" AS ""ProductoCodigo""
                                                ,""A"".""Descripcion"" AS ""ProductoDescripcion""
                                                ,""A"".""ActivoYConcentracion"" AS ""ProductoActivoConcentracion""
                                                ,""A"".""Imagen"" AS ""ProductoImagen""
			                                    ,""B"".""Stock""
			                                    ,""B"".""BodegaId""
                                                ,""F"".""NombreBodega"" AS ""BodegaNombre""
			                                    ,""B"".""UnidadMedidaVentaId""
			                                    ,""C"".""Nombre"" AS ""UnidadMedidaVentaNombre""
			                                    ,""D"".""PrecioId""
			                                    ,""E"".""NombrePrecio"" AS ""PrecioNombre""
			                                    ,""D"".""Valor"" AS ""PrecioValor""
			                                    ,""B"".""PrecioCosto"" AS ""PrecioCompra""
                                    FROM        ""Productos"" AS ""A""
                                    LEFT JOIN   ""ProductosInventario"" AS ""B"" ON ""A"".""Id"" = ""B"".""ProductoId""
                                    LEFT JOIN   ""UnidadesMedidaVenta"" AS ""C"" ON ""B"".""UnidadMedidaVentaId"" = ""C"".""Id""
                                    LEFT JOIN   ""ProductosInventarioPrecios"" AS ""D"" ON ""D"".""ProductoInventarioId"" = ""B"".""Id""  AND ""D"".""Eliminado"" = false
                                    LEFT JOIN   ""Precios"" AS ""E"" ON ""E"".""Id"" = ""D"".""PrecioId""
                                    LEFT JOIN	""Bodegas"" AS ""F"" ON ""F"".""Id"" = ""B"".""BodegaId""
                                    WHERE       (ambiente_id IS NULL OR ""A"".""AmbienteId"" = ambiente_id)
                                    AND (grupo_terapeutico_id IS NULL OR ""A"".""GrupoTProductoId"" = grupo_terapeutico_id)
                                    AND (bodega_id IS NULL OR ""B"".""BodegaId"" = bodega_id)
                                    AND (sucursal_id IS NULL OR ""F"".""SucursalId"" = sucursal_id)
                                    AND (tipo_producto_id IS NULL OR ""A"".""TipoProductoId"" = tipo_producto_id)
                                    AND ""A"".""Eliminado"" = false;

                                        END;
                        
                    
                $BODY$;
                    ";

            }
        }
        public string Sp_inventario_productos
        {
            get
            {
                return @"

                CREATE OR REPLACE FUNCTION inventario_productos(
	                    tipo_producto_id integer,
	                    grupo_terapeutico_id integer,
	                    bodega_id integer,
	                    sucursal_id integer,
	                    ambiente_id integer)
                        RETURNS TABLE(id integer, tipobodegaid integer, 
				                      viadminid integer, 
				                      tipoproductoid integer, 
				                      ambienteid integer,
				                      grupotproductoid integer, 
				                      presentacionproductoid integer, 
				                      laboratorioproductoid integer, 
				                      marcaid integer, categoriaid integer, 
				                      grupoid integer, nombreproducto text, 
				                      precio numeric, precio_2 numeric, 
				                      precio_3 numeric, precio_4 numeric, 
				                      precio_5 numeric, precio_6 numeric, 
				                      precio_7 numeric, preciocosto numeric, 
				                      stock numeric, stockinical integer, codigoreferencia text, 
				                      imagen text, descripcion text, activoyconcentracion text, 
				                      dosis text, fechavencimiento date, eliminado boolean, ubicacion text) 
                        LANGUAGE 'plpgsql'
                        COST 100
                        VOLATILE PARALLEL UNSAFE
                        ROWS 1000

                    AS $BODY$

                                    BEGIN
                                        RETURN QUERY
                                        SELECT p.""Id"" AS ""Id"",
		                                    p.""TipoBodegaId"" AS ""TipoBodegaId"",
		                                    p.""ViadminId"" AS ""ViadminId"",
		                                    p.""TipoProductoId"" AS ""TipoProductoId"",
						                    p.""AmbienteId"",
		                                    p.""GrupoTProductoId"" AS ""GrupoTProductoId"",
		                                    p.""PresentacionProductoId"" AS ""PresentacionProductoId"",
		                                    p.""LaboratorioProductoId"" AS ""LaboratorioProductoId"",
		                                    p.""MarcaId"" AS ""MarcaId"",
		                                    p.""CategoriaId"" AS ""CategoriaId"",
		                                    p.""GrupoId"" AS ""GrupoId"",
		                                    p.""NombreProducto"" AS ""NombreProducto"",
		                                    p.""Precio"" AS ""Precio"",
		                                    p.""Precio_2"" AS ""Precio_2"",
		                                    p.""Precio_3"" AS ""Precio_3"",
		                                    p.""Precio_4"" AS ""Precio_4"",
		                                    p.""Precio_5"" AS ""Precio_5"",
		                                    p.""Precio_6"" AS ""Precio_6"",
		                                    p.""Precio_7"" AS ""Precio_7"",
		                                    p.""PrecioCosto"" AS ""PrecioCosto"",
		                                    p.""Stock"" AS ""Stock"",
		                                    p.""StockInical"" AS ""StockInical"",
		                                    p.""CodigoReferencia"" AS ""CodigoReferencia"",
		                                    p.""Imagen"" AS ""Imagen"",
		                                    p.""Descripcion"" AS ""Descripcion"",
		                                    p.""ActivoYConcentracion"" AS ""ActivoYConcentracion"",
		                                    p.""Dosis"" AS ""Dosis"",
		                                    p.""FechaVencimiento"" :: DATE AS ""FechaVencimiento"",
		                                    p.""Eliminado"" AS ""Eliminado"",
		                                    p.""Ubicacion"" AS ""Ubicacion""	
                                        FROM ""Productos"" AS p
                                        LEFT JOIN ""ProductosInventario"" AS pi ON pi.""ProductoId"" = p.""Id""
                                        LEFT JOIN ""Bodegas"" AS b ON pi.""BodegaId"" = b.""Id""
                                        WHERE (tipo_producto_id IS NULL OR p.""TipoProductoId"" = tipo_producto_id)
	                                    AND (grupo_terapeutico_id IS NULL OR p.""GrupoTProductoId"" = grupo_terapeutico_id)
                                        AND (bodega_id IS NULL OR pi.""BodegaId"" = bodega_id)
                                        AND (sucursal_id IS NULL OR b.""SucursalId"" = sucursal_id)
                                        AND (ambiente_id IS NULL OR p.""AmbienteId"" = ambiente_id)
	                                    AND p.""Eliminado""=false;
                                    END;
                
                    $BODY$;
                    ";

            }
        }
        public string Sp_get_servicios
        {
            get
            {
                return @"

                CREATE OR REPLACE FUNCTION get_servicios(
                ) RETURNS TABLE (
                    Item BIGINT,
                    ServicioId INTEGER,
                    ServicioCodigo TEXT,
                    ServicioNombre TEXT,
                    ServicioDescripcion TEXT,
                    PrecioId INTEGER,
                    PrecioNombre TEXT,
                    PrecioValor NUMERIC
                ) AS $$
                BEGIN
                    RETURN QUERY
                    SELECT		ROW_NUMBER() OVER () AS ""Item""
			                    ,A.""Id"" AS ""ServicioId""
			                    ,A.""CodigoInterno"" AS ""ServicioCodigo""
			                    ,A.""NombreServicio"" AS ""ServicioNombre""
			                    ,A.""Descripcion"" AS ""ServicioDescripcion""
			                    ,B.""PrecioId""
			                    ,C.""NombrePrecio"" AS ""PrecioNombre""
			                    ,B.""Valor"" AS ""PrecioValor""
                    FROM ""Servicios"" AS A
                    LEFT JOIN ""ServiciosPrecios"" AS B ON A.""Id"" = B.""ServicioId""
                    LEFT JOIN ""Precios"" AS C ON B.""PrecioId"" = C.""Id""
                    WHERE A.""Eliminado"" = false
                    AND C.""Eliminado"" = null OR C.""Eliminado"" = false;

                END;
                        $$ LANGUAGE plpgsql;
                    ";
            }
        }
        public string Sp_auditoria_nuevo_inventario_producto
        {
            get
            {
                return @"

CREATE OR REPLACE FUNCTION public.auditoria_nuevo_inventario_producto(
	tipoproductoid integer,
	tipobodegaid integer)
    RETURNS TABLE(idproducto integer, nombreproducto text, codigoreferencia text, lote text, fecharecepcionlote date, idproductoinventario integer, stock numeric, preciocosto numeric, fechavencimiento date, nombreunidadcompra text, nombreunidadventa text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


					BEGIN

						--Si ninguno es nulo

						IF tipoProductoId IS NOT NULL AND tipoBodegaId IS NOT NULL THEN

							RETURN QUERY
								SELECT 
									p.""Id"" AS ""IdProducto"",
									p.""NombreProducto"",
									p.""CodigoReferencia"",
									pi.""Lote"",
									pi.""FechaRecepcionLote""::date,
									pi.""Id"" AS ""IdProductoInventario"",
									pi.""Stock"",
									pi.""PrecioCosto"" AS PrecioCosto,
									pi.""FechaVencimientoArticuloCompra""::date AS FechaVencimiento,
									uc.""Nombre"" AS ""NombreUnidadCompra"",
									uv.""Nombre"" AS ""NombreUnidadVenta""
								FROM 
									""Productos"" AS p
								JOIN 
									""ProductosInventario"" AS pi ON p.""Id"" = pi.""ProductoId"" AND pi.""Eliminado"" = false
								LEFT JOIN 
									""UnidadesMedidaCompra"" AS uc ON pi.""UnidadMedidaCompraId"" = uc.""Id""
								LEFT JOIN 
									""UnidadesMedidaVenta"" AS uv ON pi.""UnidadMedidaVentaId"" = uv.""Id""
								WHERE 
									(p.""TipoProductoId"" = tipoProductoId OR p.""TipoProductoId"" IS NULL) AND 
									(pi.""BodegaId"" = tipoBodegaId OR pi.""BodegaId"" IS NULL)
									and p.""Eliminado"" = false;
				
						--Tipo producto no es nulo y bodega si
						ELSIF  tipoProductoId IS NOT NULL AND tipoBodegaId IS NULL THEN
		
							RETURN QUERY
								SELECT 
									p.""Id"" AS ""IdProducto"",
									p.""NombreProducto"",
									p.""CodigoReferencia"",
									pi.""Lote"",
									pi.""FechaRecepcionLote""::date,
									pi.""Id"" AS ""IdProductoInventario"",
									pi.""Stock"",
									pi.""PrecioCosto"" AS PrecioCosto,
									pi.""FechaVencimientoArticuloCompra""::date AS FechaVencimiento,
									uc.""Nombre"" AS ""NombreUnidadCompra"",
									uv.""Nombre"" AS ""NombreUnidadVenta""
								FROM 
									""Productos"" AS p
								JOIN 
									""ProductosInventario"" AS pi ON p.""Id"" = pi.""ProductoId"" AND pi.""Eliminado"" = false
								LEFT JOIN 
									""UnidadesMedidaCompra"" AS uc ON pi.""UnidadMedidaCompraId"" = uc.""Id""
								LEFT JOIN 
									""UnidadesMedidaVenta"" AS uv ON pi.""UnidadMedidaVentaId"" = uv.""Id""
								WHERE 
									(p.""TipoProductoId"" = tipoProductoId OR p.""TipoProductoId"" IS NULL)
									and p.""Eliminado"" = false;
	
						--Tipo producto es nulo y bodega no
						ELSIF  tipoProductoId IS NULL AND tipoBodegaId IS NOT NULL THEN
	
							RETURN QUERY
								SELECT 
									p.""Id"" AS ""IdProducto"",
									p.""NombreProducto"",
									p.""CodigoReferencia"",
									pi.""Lote"",
									pi.""FechaRecepcionLote""::date,
									pi.""Id"" AS ""IdProductoInventario"",
									pi.""Stock"",
									pi.""PrecioCosto"" AS PrecioCosto,
									pi.""FechaVencimientoArticuloCompra""::date AS FechaVencimiento,
									uc.""Nombre"" AS ""NombreUnidadCompra"",
									uv.""Nombre"" AS ""NombreUnidadVenta""
								FROM 
									""Productos"" AS p
								JOIN 
									""ProductosInventario"" AS pi ON p.""Id"" = pi.""ProductoId"" AND pi.""Eliminado"" = false
								LEFT JOIN 
									""UnidadesMedidaCompra"" AS uc ON pi.""UnidadMedidaCompraId"" = uc.""Id""
								LEFT JOIN 
									""UnidadesMedidaVenta"" AS uv ON pi.""UnidadMedidaVentaId"" = uv.""Id""
								WHERE 
									(pi.""BodegaId"" = tipoBodegaId OR pi.""BodegaId"" IS NULL)
									and p.""Eliminado"" = false;
	
						--Los dos son nulos
						ELSE
		
							RETURN QUERY
								SELECT 
									p.""Id"" AS ""IdProducto"",
									p.""NombreProducto"",
									p.""CodigoReferencia"",
									pi.""Lote"",
									pi.""FechaRecepcionLote""::date,
									pi.""Id"" AS ""IdProductoInventario"",
									pi.""Stock"",
									pi.""PrecioCosto"" AS PrecioCosto,
									pi.""FechaVencimientoArticuloCompra""::date AS FechaVencimiento,
									uc.""Nombre"" AS ""NombreUnidadCompra"",
									uv.""Nombre"" AS ""NombreUnidadVenta""
								FROM 
									""Productos"" AS p
								JOIN 
									""ProductosInventario"" AS pi ON p.""Id"" = pi.""ProductoId"" AND pi.""Eliminado"" = false
								LEFT JOIN 
									""UnidadesMedidaCompra"" AS uc ON pi.""UnidadMedidaCompraId"" = uc.""Id""
								LEFT JOIN 
									""UnidadesMedidaVenta"" AS uv ON pi.""UnidadMedidaVentaId"" = uv.""Id""
									where p.""Eliminado"" = false;
						END IF;
	
					END;
					
					
				$BODY$;

				";
            }
        }
        public string Sp_get_ventas_producto_annio
        {
            get
            {
                return @"CREATE OR REPLACE FUNCTION get_ventas_producto_annio(
                    producto_id INTEGER,
                    annio_param INTEGER
                ) RETURNS TABLE (
                    Item BIGINT,
                    Annio INTEGER,
                    Mes INTEGER,
                    ProductoId INTEGER,
                    ProductoCantidad INTEGER,
                    ProductoNombre TEXT,
                    Ambiente TEXT
                ) AS $$
                BEGIN
                    RETURN QUERY
                    SELECT		ROW_NUMBER() OVER () AS Item
			                    ,CAST(EXTRACT(YEAR FROM ""FechaVenta"") AS INTEGER) AS Annio
			                    ,CAST(EXTRACT(MONTH FROM ""FechaVenta"") AS INTEGER) AS Mes
			                    ,C.""Id"" AS ProductoId
			                    ,B.""Cantidad"" AS ProductoCantidad
			                    ,C.""NombreProducto"" AS ProductoNombre
			                    ,LOWER(A.""TipoVenta"") AS Ambiente
                    FROM		""Ventas"" AS A
                    INNER JOIN	""DetalleVentas"" AS B
                    ON			A.""Id"" = B.""VentaId""
                    INNER JOIN	""Productos"" AS C
                    ON			B.""ProductoId"" = C.""Id""
                    WHERE		EXTRACT(YEAR FROM ""FechaVenta"") = annio_param
			                    AND C.""Id"" = producto_id;

                END;
                        $$ LANGUAGE plpgsql;
			";
            }
        }
        public string Sp_obtener_examenes_lab_clinicos
        {
            get
            {
                return @"CREATE OR REPLACE FUNCTION public.obtener_examenes_lab_clinicos(
								)
								RETURNS TABLE(examenid integer, examennombre text, examencodigo text, examennombremostrar text) 
								LANGUAGE 'plpgsql'
								COST 100
								VOLATILE PARALLEL UNSAFE
								ROWS 1000

							AS $BODY$

										BEGIN
											RETURN QUERY
											SELECT ""Id"" AS ""ExamenId"", ""NombreExamen"" AS ""ExamenNombre"", ""CodigoInterno"" AS ""ExamenCodigo"",CONCAT(""CodigoInterno"", ' - ', ""NombreExamen"") AS ""ExamenNombreMostrar"" FROM ""ExamenLabClinicos""
											WHERE ""Eliminado"" = false;
										END;
            
							$BODY$;
			";
            }
        }
        //Retorna concatenado todo el Script de los SP
        public string ScriptSps
        {
            get
            {
                return Sp_registrar_auditoria_producto
                        + Sp_get_inventario
                        + Sp_inventario_productos
                        + Sp_get_servicios
                        + Sp_get_ventas_producto_annio
                        + Sp_obtener_examenes_lab_clinicos
                        + Sp_auditoria_nuevo_inventario_producto
                        + Sp_consultas_productos_existentes_venta;
            }
        }
    }
}
