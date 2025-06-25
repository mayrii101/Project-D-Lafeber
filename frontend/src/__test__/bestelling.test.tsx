import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import Bestellingen from "../components/Bestellingen";
import "@testing-library/jest-dom";

describe("Bestellingen form validation", () => {
  const defaultProps = {
    filteredOrders: [],
    selectedOrder: null,
    onSelectOrder: jest.fn(),
    onBack: jest.fn(),
    onClose: jest.fn(),
    searchTerm: "",
    onSearchChange: jest.fn(),
    selectedStatus: "",
    onStatusChange: jest.fn(),
  };

  beforeEach(() => {
    render(<Bestellingen {...defaultProps} />);
  });

  it("shows error when submitting with empty fields", () => {
    fireEvent.click(screen.getByRole("button", { name: /bestelling toevoegen/i }));

    expect(screen.getByText("Klant mag niet leeg zijn.")).toBeInTheDocument();
    expect(screen.getByText("Product mag niet leeg zijn.")).toBeInTheDocument();
    expect(screen.getByText("Aantal moet een geheel getal boven 0 zijn.")).toBeInTheDocument();
    expect(screen.getByText("Verzendadres mag niet leeg zijn.")).toBeInTheDocument();
    expect(screen.getByText("Verwachte leverdatum mag niet in het verleden liggen.")).toBeInTheDocument();
  });

  it("allows valid submission", () => {
    const today = new Date().toISOString().split("T")[0];

    fireEvent.change(screen.getByLabelText(/klant/i), { target: { value: "John" } });
    fireEvent.change(screen.getByLabelText(/product/i), { target: { value: "Laptop" } });
    fireEvent.change(screen.getByLabelText(/aantal/i), { target: { value: "5" } });
    fireEvent.change(screen.getByLabelText(/verzendadres/i), { target: { value: "Main Street" } });
    fireEvent.change(screen.getByLabelText(/verwachte leverdatum/i), {
      target: { value: today },
    });

    fireEvent.click(screen.getByRole("button", { name: /bestelling toevoegen/i }));

    expect(screen.queryByText(/mag niet leeg zijn/i)).not.toBeInTheDocument();
    expect(screen.queryByText(/moet een geheel getal boven 0 zijn/i)).not.toBeInTheDocument();
  });
});
