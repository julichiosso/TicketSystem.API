<template>
  <div v-if="ticket" class="fixed inset-0 z-[110] flex items-center justify-center p-6 bg-slate-900/30 backdrop-blur-sm animate-in fade-in duration-300">
    <!-- Modal: max 90vh, flex column, sin overflow-hidden aquí para no cortar scrollbars -->
    <div class="bg-white rounded-3xl w-full max-w-4xl shadow-2xl border border-slate-200 relative flex flex-col" style="max-height:90vh; overflow:hidden;">

      <!-- Header fijo arriba -->
      <div class="px-6 pt-6 pb-4 flex-shrink-0 border-b border-slate-200">
        <div class="flex justify-between items-start">
          <div>
            <div class="flex items-center gap-2 mb-2">
              <span class="text-xs font-mono text-slate-500 bg-slate-50 px-3 py-1 rounded-full border border-slate-200">#{{ ticket.id?.substring(0, 12) }}</span>
              <StatusBadge :type="ticket.estado" :label="ticket.estado" />
              <StatusBadge :type="ticket.prioridad" :label="ticket.prioridad" />
            </div>
            <h3 class="text-2xl font-black text-slate-900 tracking-tight">{{ ticket.titulo }}</h3>
          </div>
          <button @click="$emit('close')" class="p-2 hover:bg-slate-50 rounded-xl transition-all">
            <XIcon class="w-5 h-5" />
          </button>
        </div>
      </div>

      <!-- Body: flex row, se expande y cada columna tiene su propio scroll -->
      <div class="flex flex-1 gap-6 px-6 pb-6 pt-4" style="min-height:0; overflow:hidden;">

        <!-- Columna izquierda: chat ocupa todo el alto, scroll interno manejado por ChatBox -->
        <section class="flex-1 flex flex-col" style="min-height:0;">
          <h4 class="text-xs font-bold text-slate-500 uppercase tracking-widest mb-2 flex-shrink-0">Chat de Soporte</h4>
          <ChatBox :ticketId="ticket.id" class="flex-1" style="min-height:0;" />
        </section>

        <!-- Columna derecha: ancho fijo, SCROLL PROPIO cuando el contenido no entra -->
        <section class="flex-shrink-0 space-y-4 pb-2" style="width:20rem; overflow-y:auto; min-height:0;">

          <!-- Descripción -->
          <div class="p-4 rounded-2xl border border-slate-200 bg-white/[0.02]">
            <h5 class="text-xs text-slate-500 uppercase tracking-widest mb-3">Descripción</h5>
            <p class="text-slate-700 text-sm">{{ ticket.descripcion }}</p>
          </div>

          <!-- SLA & Tiempos -->
          <div class="p-4 rounded-2xl border border-slate-200 bg-white/[0.02]">
            <h5 class="text-xs text-slate-500 uppercase tracking-widest mb-3">SLA &amp; Tiempos</h5>
            <div class="space-y-4 text-sm">
              <div class="flex justify-between items-center">
                <span class="text-slate-500">Fecha Límite</span>
                <span class="font-mono text-xs">{{ formatDate(ticket.fechaLimite) }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-slate-500">Estado SLA</span>
                <span
                  :class="ticket.slaCumplido ? 'text-emerald-400' : 'text-rose-400'"
                  class="px-2 py-0.5 rounded-full bg-white/5 text-[10px] font-black uppercase tracking-tighter"
                >
                  {{ ticket.slaCumplido ? 'En Tiempo' : 'Vencido' }}
                </span>
              </div>
              <div v-if="timeLeft" class="pt-2 border-t border-slate-200">
                <div class="text-[10px] text-slate-500 uppercase font-black tracking-widest mb-1">Tiempo Restante</div>
                <div class="text-xl font-black text-slate-900 tracking-tighter">{{ timeLeft }}</div>
              </div>
            </div>
          </div>

          <!-- Creado por -->
          <div class="p-4 rounded-2xl border border-slate-200 bg-white/[0.02] flex items-center gap-4">
            <div class="w-10 h-10 rounded-xl bg-white border border-slate-200 flex items-center justify-center flex-shrink-0 overflow-hidden">
              <span class="text-xs font-black text-blue-400">{{ (ticket.usuarioNombre || 'U')[0].toUpperCase() }}</span>
            </div>
            <div>
              <div class="text-[10px] text-slate-500 uppercase font-black tracking-widest">Creado por</div>
              <div class="font-bold text-slate-900 text-sm">{{ ticket.usuarioNombre }}</div>
              <div class="text-[10px] text-slate-500 mt-0.5">{{ formatDate(ticket.fechaCreacion) }}</div>
            </div>
          </div>

          <!-- Atendido por -->
          <div v-if="ticket.operadorAsignadoNombre" class="p-4 rounded-2xl border border-emerald-500/20 bg-emerald-500/5 flex items-center gap-4">
            <div class="w-10 h-10 rounded-xl bg-white border border-slate-200 flex items-center justify-center flex-shrink-0 overflow-hidden">
              <UserIcon class="w-5 h-5 text-emerald-500" />
            </div>
            <div>
              <div class="text-[10px] text-slate-500 uppercase font-black tracking-widest">Atendido por</div>
              <div class="font-bold text-slate-900 text-sm">{{ ticket.operadorAsignadoNombre }}</div>
              <div v-if="ticket.fechaAsignacion" class="text-[10px] text-slate-500 mt-0.5">
                Asignado el {{ formatDate(ticket.fechaAsignacion) }}
              </div>
            </div>
          </div>

          <!-- Timeline -->
          <div class="p-4 rounded-2xl border border-slate-200 bg-white/[0.02]">
            <h5 class="text-xs text-slate-500 uppercase tracking-widest mb-3">Timeline</h5>
            <div class="space-y-3">
              <div
                v-for="step in timeline"
                :key="step.key"
                class="p-2 rounded-lg border text-xs"
                :class="step.active ? 'border-indigo-500/40 bg-indigo-500/10' : 'border-slate-200 bg-white/[0.02] opacity-60'"
              >
                <div class="flex items-center justify-between gap-2">
                  <span class="font-semibold truncate">{{ step.label }}</span>
                  <span class="text-slate-500 whitespace-nowrap">{{ step.time }}</span>
                </div>
              </div>
            </div>
          </div>

        </section>
      </div>

    </div>
  </div>
</template>

<script setup>
import { computed, ref, onMounted, onUnmounted } from 'vue';
import { XIcon, UserIcon } from 'lucide-vue-next';
import StatusBadge from './StatusBadge.vue';
import ChatBox from './ChatBox.vue';
import { useSettingsStore } from '../store/settings';

const settingsStore = useSettingsStore();

const props = defineProps({
 ticket: Object
});

const emit = defineEmits(['close']);

const timeLeft = ref('');
let timer = null;

const updateTimeLeft = () => {
 if (!props.ticket?.fechaLimite || props.ticket.estado === 'Resuelto' || props.ticket.estado === 'Cerrado') {
 timeLeft.value = '';
 return;
 }
 
 const target = new Date(props.ticket.fechaLimite).getTime();
 const now = Date.now();
 const diff = target - now;
 
 if (diff <= 0) {
 timeLeft.value = 'VENCIDO';
 return;
 }
 
 const h = Math.floor(diff / (1000 * 60 * 60));
 const m = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
 const s = Math.floor((diff % (1000 * 60)) / 1000);
 
 timeLeft.value = `${h}h ${m}m ${s}s`;
};

onMounted(() => {
 updateTimeLeft();
 timer = setInterval(updateTimeLeft, 1000);
});

onUnmounted(() => {
 if (timer) clearInterval(timer);
});

const timeline = computed(() => {
 const estado = String(props.ticket?.estado ?? '').toLowerCase();
 const base = [
 { key: 'pendiente', label: 'Abierto', active: true, time: formatDate(props.ticket?.fechaCreacion) },
 { key: 'enproceso', label: 'En Progreso', active: ['enproceso', 'resuelto', 'cerrado', 'esperandousuario'].includes(estado), time: props.ticket?.fechaAsignacion ? formatDate(props.ticket.fechaAsignacion) : '---' },
 { key: 'resuelto', label: 'Resuelto', active: ['resuelto', 'cerrado'].includes(estado), time: props.ticket?.fechaResolucion ? formatDate(props.ticket.fechaResolucion) : '---' }
 ];
 return base;
});

function formatDate(value) {
  if (!value) return 'Sin fecha';
  let date = new Date(value);
  if (isNaN(date)) {
    return 'Invalid date';
  }

  return date.toLocaleString('es-AR', {
    timeZone: 'America/Argentina/Buenos_Aires',
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  });
}


</script>
