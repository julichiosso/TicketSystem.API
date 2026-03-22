<template>
  <div v-if="ticket"
    class="fixed inset-0 z-[110] flex items-center justify-center p-6 backdrop-blur-sm animate-in fade-in duration-200"
    :class="'bg-slate-900/30 dark:bg-black/60'">
    <div class="w-full max-w-4xl max-h-[95vh] md:max-h-[90vh] relative flex flex-col rounded-2xl border transition-colors overflow-hidden"
      :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 shadow-xl dark:bg-slate-900 dark:border-slate-800'">
      
      <div class="px-6 pt-5 pb-4 flex-shrink-0 border-b"
        :class="'border-slate-100 dark:border-slate-800'">
        <div class="flex justify-between items-start">
          <div>
            <div class="flex items-center gap-2 mb-2">
              <span class="text-xs font-mono px-2 py-0.5 rounded-md border"
                :class="'text-slate-400 bg-slate-50 dark:bg-slate-950 border-slate-200 dark:text-slate-500 dark:bg-slate-800 dark:border-slate-700'">
                #{{ ticket.id?.substring(0, 12) }}
              </span>
              <StatusBadge :type="ticket.status" :label="ticket.status" />
              <StatusBadge :type="ticket.priority" :label="ticket.priority" />
            </div>
            <h3 class="text-xl font-black tracking-tight"
              :class="'text-slate-900 dark:text-white'">
              {{ ticket.title }}
            </h3>
          </div>
          <button @click="$emit('close')"
            class="p-2 rounded-lg transition-all"
            :class="'hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-500 dark:hover:bg-slate-800 dark:text-slate-400'">
            <XIcon class="w-5 h-5" />
          </button>
        </div>
      </div>
      
      <div class="flex flex-col md:flex-row flex-1 gap-0 overflow-y-auto md:overflow-hidden" style="min-height: 0;">
        
        <div class="flex-1 flex flex-col p-5 border-slate-100 dark:border-slate-800"
          style="min-height: 0;">
          <p class="text-[10px] font-black uppercase tracking-widest mb-3 flex-shrink-0"
            :class="'text-slate-400 dark:text-slate-600'">
            Support Chat
          </p>
          <ChatBox :ticketId="ticket.id" class="flex-1" style="min-height: 0;" />
        </div>
        
        <div class="flex-shrink-0 flex flex-col gap-3 p-5 overflow-y-auto custom-scrollbar w-full md:w-[18rem] border-t md:border-t-0 md:border-l border-slate-100 dark:border-slate-800"
          style="min-height: 0;">
          
          <div class="rounded-xl border p-4 transition-colors"
            :class="'bg-slate-50 dark:bg-slate-950 border-slate-200 dark:bg-slate-800/50 dark:border-slate-700'">
            <p class="text-[10px] font-black uppercase tracking-widest mb-2"
              :class="'text-slate-400 dark:text-slate-600'">Description</p>
            <p class="text-sm leading-relaxed"
              :class="'text-slate-600 dark:text-slate-300'">
              {{ ticket.description }}
            </p>
          </div>
          
          <div class="rounded-xl border p-4 transition-colors"
            :class="'bg-slate-50 dark:bg-slate-950 border-slate-200 dark:bg-slate-800/50 dark:border-slate-700'">
            <p class="text-[10px] font-black uppercase tracking-widest mb-3"
              :class="'text-slate-400 dark:text-slate-600'">SLA & Timings</p>
            <div class="space-y-2 text-sm">
              <div class="flex justify-between items-center">
                <span :class="'text-slate-400 dark:text-slate-500'">Deadline</span>
                <span class="font-mono text-xs"
                  :class="'text-slate-700 dark:text-slate-300'">
                  {{ formatDate(ticket.deadline) }}
                </span>
              </div>
              <div class="flex justify-between items-center">
                <span :class="'text-slate-400 dark:text-slate-500'">SLA Status</span>
                <span class="text-[10px] font-black uppercase px-2 py-0.5 rounded-full"
                  :class="ticket.slaComplied
                    ? 'text-emerald-400 bg-emerald-500/10'
                    : 'text-rose-400 bg-rose-500/10'">
                  {{ ticket.slaComplied ? 'On Time' : 'Overdue' }}
                </span>
              </div>
              <div v-if="timeLeft" class="pt-2 border-t"
                :class="'border-slate-200 dark:border-slate-700'">
                <p class="text-[10px] uppercase tracking-widest mb-1"
                  :class="'text-slate-400 dark:text-slate-600'">Time Left</p>
                <p class="text-lg font-black tracking-tighter"
                  :class="'text-slate-900 dark:text-white'">
                  {{ timeLeft }}
                </p>
              </div>
            </div>
          </div>
          
          <div class="rounded-xl border p-4 flex items-center gap-3 transition-colors"
            :class="'bg-slate-50 dark:bg-slate-950 border-slate-200 dark:bg-slate-800/50 dark:border-slate-700'">
            <div class="w-9 h-9 rounded-lg flex items-center justify-center flex-shrink-0 font-black text-sm text-white bg-blue-600">
              {{ (ticket.userName || 'U')[0].toUpperCase() }}
            </div>
            <div>
              <p class="text-[10px] uppercase tracking-widest font-black"
                :class="'text-slate-400 dark:text-slate-600'">Created by</p>
              <p class="font-bold text-sm"
                :class="'text-slate-900 dark:text-white'">
                {{ ticket.userName }}
              </p>
              <p class="text-[10px]"
                :class="'text-slate-400 dark:text-slate-600'">
                {{ formatDate(ticket.createdAt) }}
              </p>
            </div>
          </div>
          
          <div v-if="ticket.assignedOperatorName"
            class="rounded-xl border border-emerald-500/20 bg-emerald-500/5 p-4 flex items-center gap-3">
            <div class="w-9 h-9 rounded-lg bg-emerald-500/10 border border-emerald-500/20 flex items-center justify-center flex-shrink-0">
              <UserIcon class="w-4 h-4 text-emerald-400" />
            </div>
            <div>
              <p class="text-[10px] uppercase tracking-widest font-black text-emerald-500/60">Handled by</p>
              <p class="font-bold text-sm"
                :class="'text-slate-900 dark:text-white'">
                {{ ticket.assignedOperatorName }}
              </p>
              <p v-if="ticket.assignedAt" class="text-[10px]"
                :class="'text-slate-400 dark:text-slate-600'">
                {{ formatDate(ticket.assignedAt) }}
              </p>
            </div>
          </div>
          
          <div class="rounded-xl border p-4 transition-colors"
            :class="'bg-slate-50 dark:bg-slate-950 border-slate-200 dark:bg-slate-800/50 dark:border-slate-700'">
            <p class="text-[10px] font-black uppercase tracking-widest mb-3"
              :class="'text-slate-400 dark:text-slate-600'">Timeline</p>
            <div class="space-y-2">
              <div v-for="step in timeline" :key="step.key"
                class="flex items-center gap-3 p-2 rounded-lg border text-xs transition-colors"
                :class="step.active
                  ? settingsStore.isDark
                    ? 'border-blue-500/30 bg-blue-500/10 text-white'
                    : 'border-blue-200 bg-blue-50 text-slate-900 dark:text-white'
                  : settingsStore.isDark
                    ? 'border-slate-700 text-slate-600'
                    : 'border-slate-200 dark:border-slate-700 text-slate-400 opacity-60'">
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
  if (!props.ticket?.deadline ||
      props.ticket.status === 'Resolved' ||
      props.ticket.status === 'Closed') {
    timeLeft.value = ''; return;
  }
  const diff = new Date(props.ticket.deadline).getTime() - Date.now();
  if (diff <= 0) { timeLeft.value = 'OVERDUE'; return; }
  const h = Math.floor(diff / 3600000);
  const m = Math.floor((diff % 3600000) / 60000);
  const s = Math.floor((diff % 60000) / 1000);
  timeLeft.value = `${h}h ${m}m ${s}s`;
};

onMounted(() => { updateTimeLeft(); timer = setInterval(updateTimeLeft, 1000); });
onUnmounted(() => { if (timer) clearInterval(timer); });

const timeline = computed(() => {
  const status = String(props.ticket?.status ?? '').toLowerCase();
  return [
    { key: 'pending', label: 'Open',     active: true, time: formatDate(props.ticket?.createdAt) },
    { key: 'inprogress', label: 'In Progress', active: ['inprogress','resolved','closed','waitingforuser','waitinguser'].includes(status), time: props.ticket?.assignedAt ? formatDate(props.ticket.assignedAt) : '---' },
    { key: 'resolved',  label: 'Resolved',    active: ['resolved','closed'].includes(status), time: props.ticket?.resolvedAt ? formatDate(props.ticket.resolvedAt) : '---' },
  ];
});

function formatDate(value) {
  if (!value) return '---';
  const str = value.toString();
  const normalized = str.endsWith('Z') || str.includes('+') ? str : str + 'Z';
  const date = new Date(normalized);
  if (isNaN(date)) return '---';
  return date.toLocaleString('en-US', {
    year: 'numeric', month: '2-digit', day: '2-digit',
    hour: '2-digit', minute: '2-digit'
  });
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(100,100,100,0.15); border-radius: 10px; }
</style>
