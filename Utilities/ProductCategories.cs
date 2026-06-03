using System;
using System.Collections.Generic;
using System.Linq;

namespace STOCKPAP.Utilities
{
    public static class ProductCategories
    {
        private static readonly Dictionary<string, Dictionary<string, string[]>> Catalog =
            new Dictionary<string, Dictionary<string, string[]>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Escritura"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Lapices"] = new[] { "Numero 2", "HB", "2B", "4B", "6B", "Bicolor", "Carpintero", "Mecanico 0.5 mm", "Mecanico 0.7 mm", "Mecanico 0.9 mm" },
                    ["Lapiceros"] = new[] { "Punta fina", "Punta media", "Punta gruesa", "Gel", "Tinta azul", "Tinta negra", "Tinta roja", "Retractil", "Borrable", "Multicolor" },
                    ["Plumas"] = new[] { "Fuente", "Rollerball", "Gel premium", "Caligrafia", "Tinta liquida", "Tinta permanente" },
                    ["Crayones"] = new[] { "Cera", "Jumbo", "Triangulares", "Lavables", "Neon", "Metalicos" },
                    ["Colores"] = new[] { "Madera 12 piezas", "Madera 24 piezas", "Madera 36 piezas", "Acuarelables", "Profesionales", "Bicolores" },
                    ["Marcadores"] = new[] { "Punta fina", "Punta media", "Punta gruesa", "Permanentes", "Para pizarron", "Para rotafolio", "Lavables" },
                    ["Resaltadores"] = new[] { "Amarillo", "Pastel", "Neon", "Doble punta", "Borrable", "Set surtido" },
                    ["Correctores"] = new[] { "Cinta", "Liquido", "Pluma correctora", "Refaccion" }
                },
                ["Papel"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Hojas"] = new[] { "Carta", "Oficio", "A4", "Color", "Opalina", "Reciclado", "Bond", "Ledger" },
                    ["Cuadernos"] = new[] { "Profesional", "Forma francesa", "Forma italiana", "Doble raya", "Cuadro grande", "Cuadro chico", "Raya", "Dibujo", "Espiral", "Cosido" },
                    ["Libretas"] = new[] { "Taquigrafia", "Notas", "Reportero", "Pasta dura", "Pasta flexible", "Indice" },
                    ["Cartulinas"] = new[] { "Blanca", "Color", "Metalica", "Fluorescente", "Opalina", "Bristol" },
                    ["Papeles especiales"] = new[] { "Crepe", "China", "Bond", "Fotografico", "Albanene", "Kraft", "Lustre", "Cascaron" },
                    ["Blocks"] = new[] { "Dibujo", "Marquilla", "Acuarela", "Milimetrico", "Papel mantequilla", "Notas adhesivas" }
                },
                ["Adhesivos"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Pegamentos"] = new[] { "Barra", "Liquido blanco", "Silicon frio", "Silicon caliente", "Instantaneo", "Escolar", "Profesional" },
                    ["Cintas"] = new[] { "Transparente", "Masking tape", "Doble cara", "Canela", "Aislante", "Decorativa", "Empaque", "Correctiva" },
                    ["Etiquetas"] = new[] { "Blancas", "Color", "Redondas", "Para precio", "Escolares", "Adhesivas carta", "Codigo de barras" },
                    ["Notas adhesivas"] = new[] { "Pequenas", "Medianas", "Grandes", "Banderitas", "Neon", "Pastel" }
                },
                ["Organizacion"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Carpetas"] = new[] { "Carta", "Oficio", "Con broche", "Con liga", "Argolla 1 pulgada", "Argolla 2 pulgadas", "Argolla 3 pulgadas", "Expediente" },
                    ["Archivadores"] = new[] { "Acordeon", "Caja archivo", "Revistero", "Folder colgante", "Separadores", "Protector de hojas" },
                    ["Folders"] = new[] { "Manila carta", "Manila oficio", "Color carta", "Color oficio", "Con broche", "Plastificado" },
                    ["Agendas"] = new[] { "Diaria", "Semanal", "Mensual", "Ejecutiva", "Escolar", "Planeador" }
                },
                ["Oficina"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Grapado"] = new[] { "Grapadora mini", "Grapadora escritorio", "Grapas estandar", "Grapas uso pesado", "Quitagrapas" },
                    ["Clips y broches"] = new[] { "Clip chico", "Clip jumbo", "Binder clip chico", "Binder clip mediano", "Binder clip grande", "Broche baco" },
                    ["Sellos"] = new[] { "Fechador", "Numerador", "Tinta para sello", "Almohadilla", "Sello personalizado" },
                    ["Accesorios"] = new[] { "Tijera oficina", "Cutter", "Regla metalica", "Perforadora", "Charola documentos", "Portagafete" }
                },
                ["Arte y dibujo"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Pinturas"] = new[] { "Acuarela", "Acrilica", "Tempera", "Oleo", "Dactilar", "Textil" },
                    ["Pinceles"] = new[] { "Redondo", "Plano", "Abanico", "Angular", "Set escolar", "Set profesional" },
                    ["Dibujo tecnico"] = new[] { "Compas", "Transportador", "Escuadras", "Curvigrafo", "Escalimetro", "Plantillas" },
                    ["Lienzos y bases"] = new[] { "Bastidor", "Carton ilustracion", "Papel acuarela", "Papel marquilla", "Foam board" },
                    ["Manualidades"] = new[] { "Foami", "Pompones", "Limpiapipas", "Diamantina", "Ojos movibles", "Palitos madera", "Fieltro" }
                },
                ["Corte y medicion"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Tijeras"] = new[] { "Escolar punta roma", "Escolar punta fina", "Oficina", "Zurdo", "Decorativa", "Costura" },
                    ["Cutters"] = new[] { "Chico", "Grande", "Precision", "Repuestos", "Base de corte" },
                    ["Reglas"] = new[] { "15 cm", "30 cm", "50 cm", "Metalica", "Flexible", "T plastica" },
                    ["Geometria"] = new[] { "Juego geometrico", "Escuadra 45", "Escuadra 60", "Transportador 180", "Compas escolar" }
                },
                ["Tecnologia e impresion"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Tintas"] = new[] { "Canon", "Epson", "HP", "Brother", "Negra", "Color", "Botella", "Cartucho" },
                    ["Toners"] = new[] { "HP", "Brother", "Canon", "Samsung", "Negro", "Color" },
                    ["Accesorios"] = new[] { "USB", "Mouse", "Teclado", "Audifonos", "Cable USB", "Cable HDMI", "Calculadora basica", "Calculadora cientifica" },
                    ["Impresion"] = new[] { "Papel fotografico", "Mica para engargolar", "Arillo", "Pasta engargolado", "Mica termica" }
                },
                ["Empaque y regalos"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Sobres"] = new[] { "Carta", "Oficio", "Bolsa", "Manila", "Burbuja", "Invitacion", "Color" },
                    ["Bolsas"] = new[] { "Regalo chica", "Regalo mediana", "Regalo grande", "Celofan", "Kraft", "Plastica" },
                    ["Envolturas"] = new[] { "Papel regalo", "Moños", "Liston", "Tarjetas", "Cajas regalo", "Papel china" }
                },
                ["Escolar y mochilas"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Mochilas"] = new[] { "Preescolar", "Primaria", "Secundaria", "Con ruedas", "Lonchera", "Lapicera" },
                    ["Material escolar"] = new[] { "Juego geometrico", "Diccionario", "Mapas", "Monografias", "Bata escolar", "Portaminas" },
                    ["Didactico"] = new[] { "Plastilina", "Rompecabezas", "Abaco", "Loteria educativa", "Memorama", "Letras moviles" }
                },
                ["Limpieza y varios"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Limpieza"] = new[] { "Gel antibacterial", "Toallas humedas", "Panos", "Limpiador pantalla", "Borrador pizarron" },
                    ["Varios"] = new[] { "Pilas AA", "Pilas AAA", "Llavero", "Portamonedas", "Lupa", "Cordones", "Otros" }
                }
            };

        public static string[] Defaults => Catalog.Keys.OrderBy(c => c).ToArray();

        public static string[] GetClassifications(string category)
        {
            if (!string.IsNullOrWhiteSpace(category) && Catalog.TryGetValue(category, out var classifications))
                return classifications.Keys.OrderBy(c => c).ToArray();

            return new[] { "General" };
        }

        public static string[] GetDetails(string category, string classification)
        {
            if (!string.IsNullOrWhiteSpace(category) &&
                !string.IsNullOrWhiteSpace(classification) &&
                Catalog.TryGetValue(category, out var classifications) &&
                classifications.TryGetValue(classification, out var details))
            {
                return details.OrderBy(d => d).ToArray();
            }

            return new[] { "General" };
        }

        public static string[] Merge(IEnumerable<string> extraCategories, bool includeAll = false)
        {
            var categories = new List<string>();
            if (includeAll)
                categories.Add("Todas");

            categories.AddRange(Defaults);
            AddUnique(categories, extraCategories);

            return categories.ToArray();
        }

        public static string[] MergeClassifications(string category, IEnumerable<string> extraClassifications, bool includeAll = false)
        {
            var values = new List<string>();
            if (includeAll)
                values.Add("Todas");

            values.AddRange(GetClassifications(category));
            AddUnique(values, extraClassifications);
            return values.ToArray();
        }

        public static string[] MergeDetails(string category, string classification, IEnumerable<string> extraDetails, bool includeAll = false)
        {
            var values = new List<string>();
            if (includeAll)
                values.Add("Todos");

            values.AddRange(GetDetails(category, classification));
            AddUnique(values, extraDetails);
            return values.ToArray();
        }

        private static void AddUnique(List<string> target, IEnumerable<string> source)
        {
            if (source == null)
                return;

            foreach (var item in source)
            {
                var text = item?.Trim();
                if (!string.IsNullOrWhiteSpace(text) &&
                    !target.Any(c => string.Equals(c, text, StringComparison.OrdinalIgnoreCase)))
                {
                    target.Add(text);
                }
            }
        }
    }
}
