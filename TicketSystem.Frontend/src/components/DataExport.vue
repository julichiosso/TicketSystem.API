<template>
 <div class="japanese-box space-y-6">
 <h3 class="text-lg font-black text-slate-900 dark:text-white">Exportar Datos</h3>
 <!-- Filtros -->
 <div class="grid md:grid-cols-2 gap-4">
 <div class="space-y-2">
 <label class="text-xs font-bold text-slate-500 uppercase">Tipo de Datos</label>
 <select v-model="dataType" class="w-full bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-lg px-3 py-2 text-sm">
 <option value="tickets">Tickets</option>
 <option value="usuarios">Usuarios</option>
 <option value="auditoria">Auditoría</option>
 <option value="completo">Datos Completos</option>
 </select>
 </div>
 <div class="space-y-2">
 <label class="text-xs font-bold text-slate-500 uppercase">Rango de Fechas</label>
 <div class="flex gap-2">
 <input type="date" v-model="dateFrom" class="flex-1 bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-lg px-3 py-2 text-sm" />
 <input type="date" v-model="dateTo" class="flex-1 bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-lg px-3 py-2 text-sm" />
 </div>
 </div>
 </div>
 <!-- Seleccionar campos -->
 <div class="space-y-2">
 <label class="text-xs font-bold text-slate-500 uppercase">Campos a Incluir</label>
 <div class="grid grid-cols-2 md:grid-cols-3 gap-2">
 <label v-for="field in availableFields" :key="field" class="flex items-center gap-2 cursor-pointer hover:bg-white dark:bg-slate-900 p-2 rounded">
 <input type="checkbox" v-model="selectedFields" :value="field" class="rounded" />
 <span class="text-sm text-slate-700">{{ field }}</span>
 </label>
 </div>
 </div>
 <!-- Botones de exportación -->
 <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
 <button @click="exportExcel" class="px-3 py-2 bg-emerald-600 hover:bg-emerald-500 text-slate-900 dark:text-white rounded-lg text-sm font-bold transition flex items-center justify-center gap-2">
 <FileXlsxIcon class="w-4 h-4" />
 Excel
 </button>
 <button @click="exportPdf" class="px-3 py-2 bg-red-600 hover:bg-red-500 text-slate-900 dark:text-white rounded-lg text-sm font-bold transition flex items-center justify-center gap-2">
 <FileIcon class="w-4 h-4" />
 PDF
 </button>
 <button @click="printData" class="px-3 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded-lg text-sm font-bold transition flex items-center justify-center gap-2">
 <PrinterIcon class="w-4 h-4" />
 Imprimir
 </button>
 <button @click="exportJson" class="px-3 py-2 bg-purple-600 hover:bg-purple-500 text-slate-900 dark:text-white rounded-lg text-sm font-bold transition flex items-center justify-center gap-2">
 <CodeIcon class="w-4 h-4" />
 JSON
 </button>
 </div>
 <!-- Vista previa -->
 <div v-if="previewData.length > 0" class="space-y-2">
 <label class="text-xs font-bold text-slate-500 uppercase">Vista Previa</label>
 <div class="bg-slate-50 dark:bg-slate-950 rounded-lg p-3 max-h-48 overflow-auto border border-slate-200 dark:border-slate-700">
 <div class="text-xs text-slate-700 space-y-1">
 <div v-for="(item, idx) in previewData.slice(0, 5)" :key="idx" class="flex gap-2 pb-2 border-b border-slate-200 dark:border-slate-700 last:border-0">
 <span class="text-slate-500">{{ idx + 1 }}.</span>
 <span>{{ JSON.stringify(item).substring(0, 80) }}...</span>
 </div>
 </div>
 <div v-if="previewData.length > 5" class="text-xs text-slate-500 mt-2">+ {{ previewData.length - 5 }} más</div>
 </div>
 </div>
 </div>
</template>
<script setup>
import { ref, computed } from 'vue';
import { FileIcon, PrinterIcon, CodeIcon } from 'lucide-vue-next';
const props = defineProps({
 tickets: Array,
 usuarios: Array,
 auditLog: Array
});
const emit = defineEmits(['export']);
const dataType = ref('tickets');
const dateFrom = ref('');
const dateTo = ref('');
const selectedFields = ref(['ID', 'Título', 'Usuario', 'Estado', 'Fecha']);
const availableFields = ref(['ID', 'Título', 'Descripción', 'Usuario', 'Estado', 'Prioridad', 'Fecha', 'Comentarios']);
const previewData = computed(() => {
  let data = [];
  if (dataType.value === 'tickets' && props.tickets) {
    data = props.tickets.map(t => ({
      ID: t.id?.substring(0, 8),
      Título: t.titulo,
      Usuario: t.usuarioNombre,
      Operador: t.operadorAsignadoNombre || 'No asignado',
      Estado: t.estado,
      Prioridad: t.prioridad,
      Fecha: new Date(t.fechaCreacion).toLocaleString('es-AR')
    }));
  } else if (dataType.value === 'usuarios' && props.usuarios) {
    data = props.usuarios.map(u => ({
      ID: u.id?.substring(0, 8),
      Nombre: u.nombre,
      Email: u.email,
      Rol: u.rol
    }));
  } else if (dataType.value === 'auditoria' && props.auditLog) {
    data = props.auditLog.map(l => ({
      Fecha: new Date(l.timestamp || l.fecha || l.Fecha).toLocaleString('es-AR'),
      Mensaje: l.message || l.detalle || l.Detalle,
      Acción: l.type || l.accion || l.Accion
    }));
  }
  return data;
});
const exportExcel = () => {
  const csv = convertToCSV(previewData.value);
  downloadFile(csv, `${dataType.value}_${new Date().toISOString().split('T')[0]}.csv`, 'text/csv');
};
const exportJson = () => {
  const json = JSON.stringify(previewData.value, null, 2);
  downloadFile(json, `${dataType.value}_${new Date().toISOString().split('T')[0]}.json`, 'application/json');
};
const exportPdf = () => {
  const content = previewData.value.map(item => JSON.stringify(item)).join('\n');
  downloadFile(content, `${dataType.value}_${new Date().toISOString().split('T')[0]}.txt`, 'text/plain');
};
const printData = () => {
  const html = `
    <table border="1" cellpadding="5" cellspacing="0" style="width:100%; border-collapse: collapse; font-family: sans-serif;">
      <thead>
        <tr style="background: #f8fafc;">${Object.keys(previewData.value[0] || {}).map(f => '<th>' + f + '</th>').join('')}</tr>
      </thead>
      <tbody>
        ${previewData.value.map(item =>
          '<tr>' + Object.values(item).map(v => '<td>' + v + '</td>').join('') + '</tr>'
        ).join('')}
      </tbody>
    </table>
  `;
  const win = window.open('', '', 'width=800,height=600');
  win.document.write('<html><head><title>Exportación de Datos</title></head><body><h2 style="font-family: sans-serif;">Reporte de ' + dataType.value + '</h2>' + html + '</body></html>');
  win.document.close();
  setTimeout(() => {
    win.print();
    win.close();
  }, 500);
};
const convertToCSV = (data) => {
 if (!data || data.length === 0) return '';
 const headers = Object.keys(data[0]);
 const rows = data.map(item => headers.map(h => item[h]).join(','));
 return [headers.join(','), ...rows].join('\n');
};
const downloadFile = (content, filename, type) => {
 const blob = new Blob([content], { type });
 const url = window.URL.createObjectURL(blob);
 const a = document.createElement('a');
 a.href = url;
 a.download = filename;
 document.body.appendChild(a);
 a.click();
 window.URL.revokeObjectURL(url);
 document.body.removeChild(a);
};
const FileXlsxIcon = FileIcon;
</script>
