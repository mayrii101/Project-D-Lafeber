import React from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";

interface OrderBarChartProps {
  orders: Array<{
    orderDate: string;
  }>;
}

// Helper to group orders by month-year string and count them
const getOrderCountsByMonth = (orders: OrderBarChartProps["orders"]) => {
  const counts: Record<string, number> = {};
  orders.forEach(({ orderDate }) => {
    const date = new Date(orderDate);
    if (isNaN(date.getTime())) return; // ignore invalid dates
    // Format: "YYYY-MM" for grouping
    const monthYear = `${date.getFullYear()}-${(date.getMonth() + 1)
      .toString()
      .padStart(2, "0")}`;
    counts[monthYear] = (counts[monthYear] || 0) + 1;
  });
  // Convert to array sorted by date ascending
  return Object.entries(counts)
    .map(([monthYear, count]) => ({ monthYear, count }))
    .sort((a, b) => (a.monthYear > b.monthYear ? 1 : -1));
};

const OrderBarChart: React.FC<OrderBarChartProps> = ({ orders }) => {
  const data = getOrderCountsByMonth(orders);

  return (
    <div style={{ width: "600px", height: 400 }}>
      <ResponsiveContainer width="100%" height="100%">
        <BarChart
          data={data}
          margin={{
            top: 20,
            right: 30,
            left: 20,
            bottom: 20,
          }}
        >
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="monthYear" />
          <YAxis allowDecimals={false} />
          <Tooltip />
          <Legend />
          <Bar dataKey="count" name="Total Orders" fill="#3874ed" />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
};

export default OrderBarChart;
