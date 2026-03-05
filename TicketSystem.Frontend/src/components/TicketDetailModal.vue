<template>
  <div v-if="ticket"
    class="fixed inset-0 z-[110] flex items-center justify-center p-6 backdrop-blur-sm animate-in fade-in duration-200"
    :class="settingsStore.isDark ? 'bg-black/60' : 'bg-slate-900/30'">

    <div class="w-full max-w-4xl relative flex flex-col rounded-2xl border transition-colors"
      :class="settingsStore.isDark
        ? 'bg-slate-900 border-slate-800'
        : 'bg-white border-slate-200 shadow-xl'"
      style="max-height: 90vh; overflow: hidden;">

      <!-- Header -->
      <div class="px-6 pt-5 pb-4 flex-shrink-0 border-b"
        :class="settingsStore.isDark ? 'border-slate-800' : 'border-slate-100'">
        <div class="flex justify-between items-start">
          <div>
            <div class="flex items-center gap-2 mb-2">
              <span class="text-xs font-mono px-2 py-0.5 rounded-md border"
                :class="settingsStore.isDark
                  ? 'text-slate-500 bg-slate-800 border-slate-700'
                  : 'text-slate-400 bg-slate-50 border-slate-200'">
                #{{ ticket.id?.substring(0, 12) }}
              </span>
              <StatusBadge :type="ticket.estado" :label="ticket.estado" />
              <StatusBadge :type="ticket.prioridad" :label="ticket.prioridad" />
            </div>
            <h3 class="text-xl font-black tracking-tight"
              :class="settingsStore.isDark ? 'text-white' : 'text-slate-900'">
              {{ ticket.titulo }}
            </h3>
          </div>
          <button @click="$emit('close')"
            class="p-2 rounded-lg transition-all"
            :class="settingsStore.isDark
              ? 'hover:bg-slate-800 text-slate-400'
              : 'hover:bg-slate-100 text-slate-500'">
            <XIcon class="w-5 h-5" />
          </button>
        </div>
      </div>

      <!-- Body -->
      <div class="flex flex-1 gap-0 overflow-hidden" style="min-height: 0;">

        <!-- Chat -->
        <div class="flex-1 flex flex-col p-5 border-r"
          :class="settingsStore.isDark ? 'border-slate-800' : 'border-slate-100'"
          style="min-height: 0;">
          <p class="text-[10px] font-black uppercase tracking-widest mb-3 flex-shrink-0"
            :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
            Chat de Soporte
          </p>
          <ChatBox :ticketId="ticket.id" class="flex-1" style="min-height: 0;" />
        </div>

        <!-- Panel derecho -->
        <div class="flex-shrink-0 flex flex-col gap-3 p-5 overflow-y-auto custom-scrollbar"
          style="width: 18rem; min-height: 0;">

          <!-- Descripción -->
          <div class="rounded-xl border p-4 transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-800/50 border-slate-700' : 'bg-slate-50 border-slate-200'">
            <p class="text-[10px] font-black uppercase tracking-widest mb-2"
              :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">Descripción</p>
            <p class="text-sm leading-relaxed"
              :class="settingsStore.isDark ? 'text-slate-300' : 'text-slate-600'">
              {{ ticket.descripcion }}
            </p>
          </div>

          <!-- SLA -->
          <div class="rounded-xl border p-4 transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-800/50 border-slate-700' : 'bg-slate-50 border-slate-200'">
            <p class="text-[10px] font-black uppercase tracking-widest mb-3"
              :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">SLA & Tiempos</p>
            <div class="space-y-2 text-sm">
              <div class="flex justify-between items-center">
                <span :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">Fecha Límite</span>
                <span class="font-mono text-xs"
                  :class="settingsStore.isDark ? 'text-slate-300' : 'text-slate-700'">
                  {{ formatDate(ticket.fechaLimite) }}
                </span>
              </div>
              <div class="flex justify-between items-center">
                <span :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">Estado SLA</span>
                <span class="text-[10px] font-black uppercase px-2 py-0.5 rounded-full"
                  :class="ticket.slaCumplido
                    ? 'text-emerald-400 bg-emerald-500/10'
                    : 'text-rose-400 bg-rose-500/10'">
                  {{ ticket.slaCumplido ? 'En Tiempo' : 'Vencido' }}
                </span>
              </div>
              <div v-if="timeLeft" class="pt-2 border-t"
                :class="settingsStore.isDark ? 'border-slate-700' : 'border-slate-200'">
                <p class="text-[10px] uppercase tracking-widest mb-1"
                  :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">Tiempo Restante</p>
                <p class="text-lg font-black tracking-tighter"
                  :class="settingsStore.isDark ? 'text-white' : 'text-slate-900'">
                  {{ timeLeft }}
                </p>
              </div>
            </div>
          </div>

          <!-- Creado por -->
          <div class="rounded-xl border p-4 flex items-center gap-3 transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-800/50 border-slate-700' : 'bg-slate-50 border-slate-200'">
            <div class="w-9 h-9 rounded-lg flex items-center justify-center flex-shrink-0 font-black text-sm text-white bg-blue-600">
              {{ (ticket.usuarioNombre || 'U')[0].toUpperCase() }}
            </div>
            <div>
              <p class="text-[10px] uppercase tracking-widest font-black"
                :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">Creado por</p>
              <p class="font-bold text-sm"
                :class="settingsStore.isDark ? 'text-white' : 'text-slate-900'">
                {{ ticket.usuarioNombre }}
              </p>
              <p class="text-[10px]"
                :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
                {{ formatDate(ticket.fechaCreacion) }}
              </p>
            </div>
          </div>

          <!-- Atendido por -->
          <div v-if="ticket.operadorAsignadoNombre"
            class="rounded-xl border border-emerald-500/20 bg-emerald-500/5 p-4 flex items-center gap-3">
            <div class="w-9 h-9 rounded-lg bg-emerald-500/10 border border-emerald-500/20 flex items-center justify-center flex-shrink-0">
              <UserIcon class="w-4 h-4 text-emerald-400" />
            </div>
            <div>
              <p class="text-[10px] uppercase tracking-widest font-black text-emerald-500/60">Atendido por</p>
              <p class="font-bold text-sm"
                :class="settingsStore.isDark ? 'text-white' : 'text-slate-900'">
                {{ ticket.operadorAsignadoNombre }}
              </p>
              <p v-if="ticket.fechaAsignacion" class="text-[10px]"
                :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
                {{ formatDate(ticket.fechaAsignacion) }}
              </p>
            </div>
          </div>

          <!-- Timeline -->
          <div class="rounded-xl border p-4 transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-800/50 border-slate-700' : 'bg-slate-50 border-slate-200'">
            <p class="text-[10px] font-black uppercase tracking-widest mb-3"
              :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">Timeline</p>
            <div class="space-y-2">
              <div v-for="step in timeline" :key="step.key"
                class="flex items-center gap-3 p-2 rounded-lg border text-xs transition-colors"
                :class="step.active
                  ? settingsStore.isDark
                    ? 'border-blue-500/30 bg-blue-500/10 text-white'
                    : 'border-blue-200 bg-blue-50 text-slate-900'
                  : settingsStore.isDark
                    ? 'border-slate-700 text-slate-600'
                    : 'border-slate-200 text-slate-400 opacity-60'">
                <div class="w-1.5 h-1.5 rounded-full flex-shrink-0"
                  :class="step.active ? 'bg-blue-500' : settingsStore.isDark ? 'bg-slate-700' : 'bg-slate-300'">
                </div>
                <span class="font-semibold flex-1">{{ step.label }}</span>
                <span class="font-mono opacity-60 text-[10px]">{{ step.time }}</span>
              </div>
            </div>
          </div>

        </div>
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
const props = defineProps({ ticket: Object });
const emit  = defineEmits(['close']);

const timeLeft = ref('');
let timer = null;

const updateTimeLeft = () => {
  if (!props.ticket?.fechaLimite ||
      props.ticket.estado === 'Resuelto' ||
      props.ticket.estado === 'Cerrado') {
    timeLeft.value = ''; return;
  }
  const diff = new Date(props.ticket.fechaLimite).getTime() - Date.now();
  if (diff <= 0) { timeLeft.value = 'VENCIDO'; return; }
  const h = Math.floor(diff / 3600000);
  const m = Math.floor((diff % 3600000) / 60000);
  const s = Math.floor((diff % 60000) / 1000);
  timeLeft.value = `${h}h ${m}m ${s}s`;
};

onMounted(() => { updateTimeLeft(); timer = setInterval(updateTimeLeft, 1000); });
onUnmounted(() => { if (timer) clearInterval(timer); });

const timeline = computed(() => {
  const estado = String(props.ticket?.estado ?? '').toLowerCase();
  return [
    { key: 'pendiente', label: 'Abierto',     active: true, time: formatDate(props.ticket?.fechaCreacion) },
    { key: 'enproceso', label: 'En Progreso',  active: ['enproceso','resuelto','cerrado','esperandousuario'].includes(estado), time: props.ticket?.fechaAsignacion ? formatDate(props.ticket.fechaAsignacion) : '---' },
    { key: 'resuelto',  label: 'Resuelto',     active: ['resuelto','cerrado'].includes(estado), time: props.ticket?.fechaResolucion ? formatDate(props.ticket.fechaResolucion) : '---' },
  ];
});

function formatDate(value) {
  if (!value) return '---';
  const str = value.toString();
  const normalized = str.endsWith('Z') || str.includes('+') ? str : str + 'Z';
  const date = new Date(normalized);
  if (isNaN(date)) return '---';
  return date.toLocaleString('es-AR', {
    timeZone: 'America/Argentina/Buenos_Aires',
    year: 'numeric', month: '2-digit', day: '2-digit',
    hour: '2-digit', minute: '2-digit'
  });
}
</script>