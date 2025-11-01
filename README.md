# Trabajo Tarjeta 2025

## 📊 Cobertura de Código

Este proyecto utiliza **GitHub Actions** y **Codecov** para medir la calidad de los tests.

[![.NET](https://github.com/julian-ferrari/TrabajoTarjeta2025/actions/workflows/dotnet.yml/badge.svg)](https://github.com/julian-ferrari/TrabajoTarjeta2025/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/julian-ferrari/TrabajoTarjeta2025/branch/main/graph/badge.svg)](https://codecov.io/gh/julian-ferrari/TrabajoTarjeta2025)

### ¿Cómo funciona?

1. En cada push/PR, GitHub Actions ejecuta los tests
2. El paquete `coverlet.collector` genera reportes de cobertura
3. Los reportes se suben automáticamente a Codecov
4. Codecov analiza qué líneas de código están cubiertas por tests

### Ver reporte completo

Puedes ver el análisis detallado en: https://codecov.io/gh/julian-ferrari/TrabajoTarjeta2025

El siguiente trabajo es un enunciado iterativo. Regularmente se ampliara y/o modificara el enunciado.
<br><br>
Aclaraciones: 
- *Todos* los metodos deben estar testeados con un test unitario, aunque no se aclare explicitamente en el enunciado.
- Dentro de las posibilidades utilizar NUnit como framework de testing
- Para la nota final se tomara en cuenta no solo el codigo fuente de la implementacion, sino tambien el uso uso de Git y las herramientas que este provee como commits, ramas y tags.
- Cada clase de la implementacion y de testing debe estar en un archivo aparte.

## Iteración 1.
Escribir un programa con programación orientada a objetos que permita ilustrar el funcionamiento del transporte urbano de pasajeros de la ciudad de Rosario.
Las clases que interactúan en la simulación son: Colectivo, Tarjeta y Boleto.
Cuando un usuario viaja en colectivo con una tarjeta, obtiene un boleto como resultado de la operación colectivo.pagarCon(tarjeta);
<br><br>
Para esta iteración se consideran los siguientes supuestos:
 - No hay medio boleto de ningún tipo.
 - No hay transbordos.
 - No hay saldo negativo.
 - La tarifa básica de un pasaje es de: $1580
 - Las cargas aceptadas de tarjetas son: (2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000)
 - El límite de saldo de una tarjeta es de $40000
<br><br>
Se pide:
 - Hacer un fork del repositorio.
 - Implementar el código de las clases Tarjeta, Colectivo y Boleto.
 - Hacer que el test Tarjeta.cs funcione correctamente con todos los montos de pago listados.
 - Enviar el enlace del repositorio al mail del profesor con los integrantes del grupo: dos por grupo.

## Iteración 2.
Para esta iteración hay 3 tareas principales. Crear un issue en github copiando la descripción de cada tarea y completar cada uno en una rama diferente. Éstas serán mergeadas al validar, luego de una revisión cruzada (de ambos integrantes del grupo), que todo el código tiene sentido y está correctamente implementado.<br>
No es necesario que todo el código para un issue esté funcionando al 100% antes de mergiearlo, pueden crear pull requests que solucionen algún item particular del problema para avanzar más rápido.
Además de las tareas planteadas, cada grupo tiene tareas pendientes de la iteración antertior que debe finalizar antes de comenzar con la iteración 2. Cuando la iteración 1 esté completada, crear un [tag](https://git-scm.com/book/en/v2/Git-Basics-Tagging) llamado iteracion1: Y subirlo a github.
<br><br>
### Covertura de codigo.
Implementar Git Actions en el repositorio, la [covertura de codigo](https://about.codecov.io/) y tambien el badge.
<br><br>
### Descuento de saldos.
Cada vez que una tarjeta paga un boleto, descuenta el valor del monto gastado.
 - Si la tarjeta se queda sin saldo, la operación $colectivo->pagarCon($tarjeta) devuelve FALSE,
<br><br>   
### Saldo negativo
- Si la tarjeta se queda sin crédito, puede tener un saldo negativo de hasta $1200.
- Cuando se vuelve a cargar la tarjeta, se descuenta el saldo de lo que se haya consumido.
- Escribir un test que valide que la tarjeta no pueda quedar con menos saldo que el permitido.
- Escribir un test que valide que el saldo de la tarjeta descuenta correctamente el/los viaje/s plus otorgado/s.
<br><br>
### Franquicia de Boleto.
Existen dos tipos de franquicia en lo que refiere a tarjetas, las franquicias parciales, como el medio boleto estudiantil o el universitario, y las completas como las de jubilados(Notar que también existe boleto gratuito para estudiantes).
- Implementar cada tipo de tarjeta como una Herencia de la tarjeta original (Medio boleto estudiantil, Boleto gratuito estudiantil, Franquicia completa).
- Para esta iteración considerar simplemente que cuando se paga con una tarjeta del tipo MedioBoleto el costo del pasaje vale la mitad, independientemente de cuántas veces se use y que día de la semana sea.
- Escribir un test que valide que una tarjeta de FranquiciaCompleta siempre puede pagar un boleto.
- Escribir un test que valide que el monto del boleto pagado con medio boleto es siempre la mitad del normal..
 - Enviar el enlace del repositorio al mail del profesor con los integrantes del grupo: dos por grupo.


## Iteracion 3.
Al igual que la iteración anterior, se pide mantener la mecánica de trabajo para ir añadiendo las nuevas funcionalidades y/o modificaciones (issue, una rama específica para cada tarea y finalmente el mergeo cuando todo funcione correctamente..., etc.)
En esta iteración daremos una introducción a la manipulación de fechas y horarios. Éstos serán necesarios en esta oportunidad para realizar las modificaciones pedidas.
<br><br>
**NOTA IMPORTANTE:** Para el manejo del tiempo al pagar un boleto tienen [este ejemplo](https://github.com/mgonzalesips/ManejoDeTiempos) de como lo pueden hacer. Entiendo que el ejemplo puede no ser claro, lo veremos mas a detalle la proxima clase.

### Más datos sobre el boleto.
La clase boleto tendrá nuevos métodos que permitan conocer: (Fecha, tipo de tarjeta, línea de colectivo, total abonado, saldo e ID de la tarjeta. El boleto deberá indicar además el saldo restante en la tarjeta.
Además el boleto debera informar el monto total abonado en caso de que la tarjeta tuviera saldo negativo y eso produzca un valor final superior al valor normal de la tarifa.
Escribir los tests correspondientes a los posibles tipos de boletos a obtener según el tipo de tarjeta.
<br><br>
### Limitación en el pago de medio boletos
Para evitar el uso de una tarjeta de tipo medio boleto en más de una persona en el mismo viaje se pide que:
- Al utilizar una tarjeta de tipo medio boleto para viajar, deben pasar como mínimo 5 minutos antes de realizar otro viaje. No será posible pagar otro viaje antes de que pasen estos 5 minutos.
- Escribir un test que verifique efectivamente que no se deje marcar nuevamente al intentar realizar otro viaje en un intervalo menor a 5 minutos con la misma tarjeta medio boleto. Para el caso del medio boleto, se pueden realizar hasta dos viajes por día. El tercer viaje ya posee su valor normal.
- Escribir un test que verifique que no se puedan realizar más de dos viajes por día con medio boleto.
<br><br>
### Limitación en el pago de franquicias completas.
Para evitar el uso de una tarjeta de tipo boleto educativo gratuito en más de una persona en el mismo viaje se pide que:
- Al utilizar una tarjeta de tipo boleto educativo gratuito se pueden realizar hasta dos viajes gratis por día.
- Escribir un test que verifique que no se puedan realizar más de dos viajes gratuitos por día.
- Escribir un test que verifique que los viajes posteriores al segundo se cobran con el precio completo.

### Saldo de la tarjeta

Una tarjeta SUBE no puede almacenar más de 56000 pesos. Por lo tanto, cuando se realiza una carga que haga que se supere este límite, se deberá acreditar la carga en la tarjeta hasta alcanzar el monto máximo permitido y el monto restante se deberá dejar pendiente de acreditación. Luego ese saldo pendiente se acredita a medida que se usa la tarjeta.

- Crear el método `AcreditarCarga`.
- Modificar la función para cargar la tarjeta añadiendo esta funcionalidad.
- Escribir un test que valide que si a una tarjeta se le carga un monto que supere el máximo permitido, se acredite el saldo hasta alcanzar el máximo (56000) y que el excedente quede almacenado y pendiente de acreditación.
- Escribir un test que valide que luego de realizar un viaje, verifique si hay saldo pendiente de acreditación y recargue la tarjeta hasta llegar al máximo nuevamente.

## Iteración 4

### Boleto de uso frecuente

Las tarjetas SUBE cuentan con el boleto de uso frecuente. Este es un beneficio que aplica un descuento al monto del boleto dependiendo de cuántos viajes se hagan.

- Del viaje 1 al 29: Tarifa normal.
- Del viaje 30 al 59: 20% de descuento.
- Del viaje 60 al 80: 25% de descuento.
- Del viaje 81 en adelante: Tarifa normal.

<br>

La cantidad de viajes se cuenta del primer al último día de cada mes. Este beneficio se aplicará *sólo* sobre las tarjetas normales.

- Implementar esta nueva funcionalidad.
- Escribir los tests correspondientes para validar el correcto funcionamiento del código.

### Franquicias

Todas las franquicias (medio boleto estudiantil, jubilado, medio boleto universitario y boleto educativo gratuito) solo pueden utilizarse en una determinada franja horaria:

- Lunes a viernes de 6 a 22.

Fuera de este intervalo de tiempo no es posible pagar con ninguna de estas franquicias.

- Escribir tests que validen que no se puedan realizar viajes fuera de la franja horaria y/o días correspondientes.

### Líneas interurbanas

En nuestra ciudad existen diversas líneas de colectivo. Algunas solo viajan dentro de la ciudad pero otras van hacia la zona metropolitana de Rosario (ej: Gálvez, Baigorria). Estas líneas tienen otra tarifa.

- Implementar las líneas de colectivo interurbanas.
- La tarifa interurbana es de: $3000.

Las líneas de colectivo interurbanas admiten todas las franquicias y siguen los mismos criterios que las líneas de colectivo urbanas.

### Trasbordos

Un usuario de la tarjeta SUBE puede realizar trasbordos entre colectivos sin costo de acuerdo a las siguientes condiciones:

- El usuario puede realizar trasbordos sin límite en un plazo de 1 hora desde que se pagó el primer boleto, pero el trasbordo debe ser entre líneas distintas.
- Los trasbordos se pueden realizar de lunes a sábado de 7:00 a 22:00.
- Todas las tarjetas pueden realizar trasbordos.
- Cuando se paga un trasbordo se indica también en el boleto.
