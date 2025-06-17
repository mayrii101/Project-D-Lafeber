import React from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from "recharts";

interface OrderGraphProps {
  data: { name: string; value: number }[];
}

const OrderGraph: React.FC<OrderGraphProps> = ({ data }) => {
  return (
    <div
      style={{
        width: "100%",
        height: 350,
        marginTop: "50px", // zorgt ervoor dat de grafiek iets lager staat
        padding: "10px",
        backgroundColor: "#fff",
        borderRadius: "12px",
        boxShadow: "0 4px 12px rgba(0, 0, 0, 0.08)",
      }}
    >
      <ResponsiveContainer>
        <BarChart
          data={data}
          margin={{ top: 10, right: 30, left: 20, bottom: 5 }}
        >
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="name" />
          <YAxis allowDecimals={false} />
          <Tooltip />
          <Legend />
          <Bar dataKey="value" fill="#4f46e5" name="Aantal bestellingen" />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
};

export default OrderGraph;
