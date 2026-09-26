<script setup lang="ts">
import { useTimesheetStore } from '@/stores/timesheetStore.ts';
import TimesheetsList from './TimesheetsList.vue';
import { storeToRefs } from 'pinia';
import { onMounted } from 'vue';

const timesheetStore = useTimesheetStore();
const { timesheets } = storeToRefs(timesheetStore);

onMounted(()=>{
    timesheets.value = [];
});

</script>
<template>
    <!-- todo: add history, additional widgets, etc. -->
    <TimesheetsList/>
</template>