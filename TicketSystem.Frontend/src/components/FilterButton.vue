<template>
 <div class="flex flex-wrap gap-2">
 <button
 v-for="option in options"
 :key="option.value"
 @click="selected = selected === option.value ? null : option.value"
 :class="`px-3 py-1 rounded-lg text-xs font-bold uppercase tracking-widest transition ${
 selected === option.value
 ? 'bg-blue-600 text-white shadow-lg shadow-blue-200'
 : 'bg-white dark:bg-slate-900 text-slate-500 hover:bg-white dark:bg-slate-900'
 }`"
 >
 {{ option.label }}
 </button>
 </div>
</template>
<script setup>
import { ref, watch } from 'vue';
defineProps({
 options: Array,
 modelValue: [String, Number]
});
const emit = defineEmits(['update:modelValue']);
const selected = ref(null);
watch(selected, (val) => {
 emit('update:modelValue', val);
});
</script>
