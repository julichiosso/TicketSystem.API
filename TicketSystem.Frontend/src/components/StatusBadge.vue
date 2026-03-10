<template>
 <span :class="badgeClass" class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-semibold tracking-wide">
 {{ displayLabel }}
 </span>
</template>
<script setup>
import { computed } from 'vue';
const props = defineProps({
 type: [String, Number],
 label: [String, Number]
});
const badgeClass = computed(() => {
 const typeStr = String(props.type || '').toLowerCase().replaceAll(' ', '');
 const types = {
  alta: 'bg-rose-50 text-rose-600 border border-rose-100',
 media: 'bg-amber-50 dark:bg-amber-900/20 text-amber-700 dark:text-amber-400 border border-amber-200 dark:border-amber-800/40',
 baja: 'bg-emerald-50 text-emerald-600 border border-emerald-100',
 pendiente: 'bg-blue-50 text-blue-600 border border-blue-100',
 enproceso: 'bg-indigo-50 text-indigo-600 border border-indigo-100',
 resuelto: 'bg-emerald-50 text-emerald-600 border border-emerald-100',
 cerrado: 'bg-slate-50 dark:bg-slate-950 text-slate-500 border border-slate-100 dark:border-slate-800/80',
 esperandousuario: 'bg-amber-50 dark:bg-amber-900/20 text-amber-700 dark:text-amber-400 border border-amber-200 dark:border-amber-800/40 sla-pulse',
 waitinguser: 'bg-amber-50 dark:bg-amber-900/20 text-amber-700 dark:text-amber-400 border border-amber-200 dark:border-amber-800/40 sla-pulse',
 high: 'bg-rose-50 text-rose-600 border border-rose-100',
 medium: 'bg-amber-50 dark:bg-amber-900/20 text-amber-700 dark:text-amber-400 border border-amber-200 dark:border-amber-800/40',
 low: 'bg-emerald-50 text-emerald-600 border border-emerald-100',
 open: 'bg-blue-50 text-blue-600 border border-blue-100',
 inprogress: 'bg-indigo-50 text-indigo-600 border border-indigo-100',
 resolved: 'bg-emerald-50 text-emerald-600 border border-emerald-100',
 closed: 'bg-slate-50 dark:bg-slate-950 text-slate-500 border border-slate-100 dark:border-slate-800/80',
 0: 'bg-blue-50 text-blue-600 border border-blue-100',
 1: 'bg-indigo-50 text-indigo-600 border border-indigo-100',
 2: 'bg-emerald-50 text-emerald-600 border border-emerald-100',
 3: 'bg-slate-50 dark:bg-slate-950 text-slate-500 border border-slate-100 dark:border-slate-800/80',
 4: 'bg-amber-50 dark:bg-amber-900/20 text-amber-700 dark:text-amber-400 border border-amber-200 dark:border-amber-800/40 sla-pulse'
 };
 return types[typeStr] || 'bg-slate-500/10 text-slate-500 border border-slate-500/20';
});
const displayLabel = computed(() => {
 const labelStr = String(props.label ?? props.type ?? '').toLowerCase().replaceAll(' ', '');
 const translations = {
 open: 'Abierto',
 inprogress: 'En Progreso',
 resolved: 'Resuelto',
 closed: 'Cerrado',
 waitinguser: 'En espera de usuario',
 pendiente: 'Abierto',
 enproceso: 'En Progreso',
 resuelto: 'Resuelto',
 cerrado: 'Cerrado',
 esperandousuario: 'En espera de usuario',
 alta: 'Alta',
 media: 'Media',
 baja: 'Baja',
 high: 'Alta',
 medium: 'Media',
 low: 'Baja',
 0: 'Abierto',
 1: 'En Progreso',
 2: 'Resuelto',
 3: 'Cerrado',
 4: 'En espera de usuario'
 };
 return translations[labelStr] || props.label || props.type;
});
</script>
