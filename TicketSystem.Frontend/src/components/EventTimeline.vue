<template>
 <div class="space-y-4">
 <h4 class="text-sm font-bold text-slate-900 uppercase tracking-widest mb-4">Historial de Cambios</h4>
 <div class="space-y-3">
 <div v-for="(event, idx) in events" :key="idx" class="flex gap-4">
 <div class="flex flex-col items-center">
 <div :class="`w-3 h-3 rounded-full ${event.color}`"></div>
 <div v-if="idx < events.length - 1" class="w-0.5 h-8 bg-slate-700 mt-2"></div>
 </div>
 <div class="pb-4">
 <p class="text-sm font-semibold text-slate-900">{{ event.title }}</p>
 <p class="text-xs text-slate-500">{{ formatTime(event.timestamp) }}</p>
 </div>
 </div>
 </div>
 </div>
</template>

<script setup>
defineProps({
 events: Array
});

const formatTime = (timestamp) => {
 const date = new Date(timestamp);
 return date.toLocaleString('es-ES', {
 month: 'short',
 day: 'numeric',
 hour: '2-digit',
 minute: '2-digit'
 });
};
</script>
