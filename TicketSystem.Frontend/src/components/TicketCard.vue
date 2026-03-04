<template>
  <div class="group relative bg-white border border-slate-100 shadow-sm rounded-2xl p-6 transition-all duration-300 hover:shadow-md hover:border-blue-100 active:scale-[0.99] hover:translate-y-[-2px]">
 <!-- Status & Priority Badge -->
 <div class="flex items-center justify-between mb-4">
 <div class="flex gap-2">
 <StatusBadge :type="ticket.estado" :label="ticket.estado" />
 <StatusBadge :type="ticket.prioridad" :label="ticket.prioridad" />
 </div>
  <div class="text-[10px] font-medium text-slate-400 bg-slate-50 px-2 py-1 rounded-md border border-slate-100">
  #{{ ticket.id?.substring(0, 8) }}
  </div>
 </div>

 <!-- Title & Content -->
  <h3 class="text-slate-800 font-bold text-base mb-2 group-hover:text-blue-600 transition-colors line-clamp-1">
  {{ ticket.titulo }}
  </h3>
 <p class="text-slate-500 text-sm line-clamp-2 mb-6 leading-relaxed">
 {{ ticket.descripcion }}
 </p>

 <!-- Footer -->
  <div class="flex items-center justify-between pt-4 border-t border-slate-50">
 <div class="flex items-center gap-3">
 <div class="w-8 h-8 rounded-full bg-white/5 border border-slate-200 flex items-center justify-center text-xs font-bold text-primary">
 {{ (ticket.usuarioNombre || 'U')[0] }}
 </div>
 <div class="text-xs">
 <div class="text-slate-700 font-semibold">{{ ticket.usuarioNombre || 'Usuario' }}</div>
 <div class="text-slate-500">{{ formatDate(ticket.fechaCreacion) }}</div>
 </div>
 </div>
 
 <!-- Actions Slot -->
 <slot name="actions"></slot>
 </div>
 
 </div>
</template>

<script setup>
import StatusBadge from './StatusBadge.vue';

defineProps({
 ticket: Object
});

const formatDate = (dateString) => {
 if (!dateString) return '';
 const d = typeof dateString === 'string' || typeof dateString === 'number' ? new Date(dateString) : dateString;
 if (!(d instanceof Date) || isNaN(d)) return '';
 return d.toLocaleDateString('es-AR', {
 day: '2-digit',
 month: 'short',
 timeZone: 'America/Argentina/Buenos_Aires'
 });
};
</script>
